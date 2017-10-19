﻿using Bot.Data;
using Bot.Models.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bot.Worker.Models
{
    [Serializable]
    public class EngineState
    {
        private object baton = new object();

        public IDictionary<Guid, CommuterRequestProcessModel> CommuterRequestProcessTable { get; private set; }

        public IDictionary<Guid, Guid> PoolerCommuterMapping { get; private set; }
        public IDictionary<Guid, Trip> OngoingTrips { get; private set; }

        public EngineState()
        {
            CommuterRequestProcessTable = new Dictionary<Guid, CommuterRequestProcessModel>();
            PoolerCommuterMapping = new Dictionary<Guid, Guid>();
        }

        public MethodResponse AddCommuterToState(TripRequest tripRequest, IList<Coordinate> route)
        {
            try
            {
                var trip = new Trip
                {
                    GoingTo = tripRequest.GoingTo,
                    Owner = tripRequest.Commuter,
                    Passengers = new List<Commuter>(tripRequest.Commuter.Vehicle.MaxPassengerCount)
                };
                
                lock (baton)
                {
                    if (CommuterRequestProcessTable.Keys.Contains(tripRequest.Commuter.CommuterId))
                    {
                        return new MethodResponse(true);
                    }
                    CommuterRequestProcessTable.Add(tripRequest.Commuter.CommuterId,
                        new CommuterRequestProcessModel(trip, route));
                }
                return new MethodResponse(true);

            }
            catch (Exception ex)
            {
                return new MethodResponse(false, ResponseCodes.InvalidInputParameter, ex.Message);
            }
        }

        public MethodResponse RemoveTripFromState(Commuter commuter)
        {
            Trip trip;

            lock (baton)
            {
                var tripIsInProcess = CommuterRequestProcessTable.ContainsKey(commuter.CommuterId);
                if (tripIsInProcess)
                {
                    CommuterRequestProcessTable.Remove(commuter.CommuterId);
                    return new MethodResponse { IsSuccess = true };
                }
            }
            lock (baton)
            {
                if (!OngoingTrips.TryGetValue(commuter.CommuterId, out trip) || trip.Passengers.Count != 0)
                {
                    return new MethodResponse { IsSuccess = false };
                }
                OngoingTrips.Remove(commuter.CommuterId);
            }
            return new MethodResponse { IsSuccess = true };
        }

        public MethodResponse RemovePoolerRequestFromState(Guid tripOwnerId, Guid poolerId)
        {
            CommuterRequestProcessModel processModel;
            lock (baton)
            {
                if (CommuterRequestProcessTable.TryGetValue(tripOwnerId, out processModel))
                {
                    var pooler = processModel.PoolerRequests
                        .FirstOrDefault(x => x.TripRequest.Commuter.CommuterId == poolerId && x.Status != TripRequestStatus.OnTheWay);
                    if (pooler != null)
                    {
                        processModel.PoolerRequests.Remove(pooler);
                    }
                    return new MethodResponse { IsSuccess = true };
                }
            }
            Trip ongoingTrip;
            lock (baton)
            {
                if (OngoingTrips.TryGetValue(tripOwnerId, out ongoingTrip))
                {
                    var pooler = ongoingTrip.Passengers.FirstOrDefault(x => x.CommuterId == poolerId);
                    if (pooler != null)
                    {
                        ongoingTrip.Passengers.Remove(pooler);
                        return new MethodResponse { IsSuccess = true };
                    }
                }
            }
            return new MethodResponse { IsSuccess = false };
        }

        public EngineState Clone()
        {
            EngineState stateClone;

            lock (baton)
            {
                using (var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, this);
                    ms.Position = 0;

                    stateClone = (EngineState)formatter.Deserialize(ms);
                }
            }
            return stateClone;
        }

        public void StartTrip(TripRequestInProcess poolRequest)
        {
            lock(baton)
            {
                // one optimization could be to use ref of objects so updating them becomes easy
                var poolId = poolRequest.TripRequest.Commuter.CommuterId;
                var tripOwnerId = PoolerCommuterMapping[poolId];
                var poolRequests = CommuterRequestProcessTable[tripOwnerId].PoolerRequests;
                var pool = poolRequests.FirstOrDefault(x => x.TripRequest.Commuter.CommuterId == poolId);
                pool.Status = TripRequestStatus.OnTheWay;
            }
        }
    }
}
