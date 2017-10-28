using Bot.Data;
using Bot.External;
using Bot.MessagingFramework;
using Bot.Models.Internal;
using Bot.Worker.Messages;
using Bot.Worker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Worker.Core
{
    internal class EngineCoreSingleRequest
    {
        private EngineState _engineState;

        public EngineCoreSingleRequest()
        {
            _engineState = EngineState.Instance;
        }
        public EngineState EngineState
        {
            get
            {
                return _engineState?.Clone();
            }
        }

        // todo:should return the data for which poolingengine could send requests messages to the customers.
        public MethodResponse ProcessRequests(ProcessTripOwnerRequestMessage message)
        {
            AddCommuterRequestsToState(message.TripOwnerRequest);

            var process = _engineState.CommuterRequestProcessTable.FirstOrDefault(x => x.Key == message.TripOwnerRequest.Commuter.CommuterId);
            
            AddPoolersRequestsToState(process.Value);

            return new MethodResponse { IsSuccess = true };
        }

        private void AddCommuterRequestsToState(TripRequest tripOwnerRequest)
        {
            var maps = new Maps();
            IList<Coordinate> route = maps.GetRoute(tripOwnerRequest);

            if (route == null || route.Count == 0)
            {
                return; // we need to put this in retry list
            }

            if (!_engineState.AddCommuterToState(tripOwnerRequest, route).IsSuccess)
            {
                return;
            }
            TripRequestManager.UpdateRequestStatus(tripOwnerRequest, RequestStatus.InProcess);

            MessageBus.Instance.Publish(new VehicleOwnerAddedToStateMessage
            {
                VehicleOwner = tripOwnerRequest.Commuter,
                TripRequest = tripOwnerRequest
            });
        }

        private MethodResponse AddPoolersRequestsToState(CommuterRequestProcessModel commuterRequestProcess)
        {
            int numberOfPoolersNeeded = commuterRequestProcess.Trip.Owner.Vehicle.RemainingSeats
                - commuterRequestProcess.PoolerRequests.Where(x => x.Status == TripRequestStatus.Waiting).Count();

            while (numberOfPoolersNeeded > 0 && !commuterRequestProcess.LastNodeReached)
            {
                var currentNode = commuterRequestProcess.CurrentNode;
                IList<TripRequest> poolersForCurrentNode = TripRequestManager.GetInitializedPoolerRequestsForCoordinate(currentNode).ToList();

                foreach (var poolRequest in poolersForCurrentNode)
                {
                    if (commuterRequestProcess.PoolerRequests.Any(x => x.TripRequest.Commuter.CommuterId == poolRequest.Commuter.CommuterId))
                        continue;
                    var tripRequestInProcess = new TripRequestInProcess { TripRequest = poolRequest, Status = TripRequestStatus.Waiting };
                    commuterRequestProcess.PoolerRequests.Add(tripRequestInProcess);
                    _engineState.PoolerCommuterMapping.Add(poolRequest.Commuter.CommuterId,
                        commuterRequestProcess.Trip.Owner.CommuterId);
                    TripRequestManager.UpdateRequestStatus(poolRequest, RequestStatus.Waiting);
                    numberOfPoolersNeeded--;

                    MessageBus.Instance.Publish(new PoolerAddedToStateMessage
                    {
                        Pooler = poolRequest.Commuter,
                        TripRequest = poolRequest,
                        VehicleOwner = commuterRequestProcess.Trip.Owner
                    });

                    if (numberOfPoolersNeeded <= 0)
                        break;
                }
            }
            return new MethodResponse(true);
        }

        public MethodResponse AddPoolersToTrip(TripRequest commuterRequest, int[] poolerIndices)
        {
            CommuterRequestProcessModel commuterRequestProcess = _engineState.CommuterRequestProcessTable[commuterRequest.Commuter.CommuterId];

            IList<TripRequest> acceptedPoolerRequests = AddPoolersToTripAndUpdatePoolerStatus(poolerIndices, commuterRequestProcess);

            RemoveAcceptedPoolersFromProcessTable(commuterRequestProcess, acceptedPoolerRequests);

            SetRejectedPoolersStateToInitialized(commuterRequestProcess);

            if (commuterRequest.Commuter.Vehicle.RemainingSeats > 0)
            {
                AddPoolersRequestsToState(commuterRequestProcess);
                return new MethodResponse(true, ResponseCodes.TripDidNotStart, string.Empty);
            }

            MessageBus.Instance.Publish(new TripStartedMessage { Trip = commuterRequestProcess.Trip });
            return new MethodResponse { IsSuccess = true };
        }

        public MethodResponse StartTrip(TripRequest commuterRequest)
        {
            CommuterRequestProcessModel commuterRequestProcess = _engineState.CommuterRequestProcessTable[commuterRequest.Commuter.CommuterId];

            foreach (var poolRequest in commuterRequestProcess.PoolerRequests.Where(x => x.Status == TripRequestStatus.Waiting))
            {
                // enginecore should only tell which request manager that the pooler doesn't need to wait anymore
                // manager should decide which state to put the pooler in
                TripRequestManager.UpdateRequestStatus(poolRequest.TripRequest, RequestStatus.InProcess);
            }
            _engineState.OngoingTrips.Add(commuterRequest.Commuter.CommuterId, commuterRequestProcess.Trip);
            _engineState.CommuterRequestProcessTable.Remove(commuterRequest.Commuter.CommuterId);
            TripRequestManager.UpdateRequestStatus(commuterRequest, RequestStatus.InTrip);

            return new MethodResponse(true, ResponseCodes.TripStarted, string.Empty);
        }

        private void SetRejectedPoolersStateToInitialized(CommuterRequestProcessModel commuterRequestProcess)
        {
            foreach (var poolRequest in commuterRequestProcess
                .PoolerRequests.Where(x => x.Status == TripRequestStatus.Requested))
            {
                TripRequestManager.UpdateRequestStatus(poolRequest.TripRequest, RequestStatus.Initialized);
                commuterRequestProcess.PoolerRequests.Remove(poolRequest);
            }
        }

        private void RemoveAcceptedPoolersFromProcessTable(CommuterRequestProcessModel commuterRequestProcess, IList<TripRequest> acceptedPoolerRequests)
        {
            foreach (var request in acceptedPoolerRequests)
            {
                var tripInRequest = commuterRequestProcess.PoolerRequests
                    .First(x => x.TripRequest.Commuter.CommuterId == request.Commuter.CommuterId);
                commuterRequestProcess.PoolerRequests.Remove(tripInRequest);
            }
        }

        private IList<TripRequest> AddPoolersToTripAndUpdatePoolerStatus(int[] poolerIndices, CommuterRequestProcessModel commuterRequestProcess)
        {
            var acceptedPoolerRequests = new List<TripRequest>();
            foreach (var index in poolerIndices)
            {
                acceptedPoolerRequests.Add(commuterRequestProcess.PoolerRequests[index].TripRequest);
                commuterRequestProcess.Trip.AddPassenger(commuterRequestProcess.PoolerRequests[index].TripRequest.Commuter);
                TripRequestManager.UpdateRequestStatus(commuterRequestProcess.PoolerRequests[index].TripRequest, RequestStatus.InTrip);

                MessageBus.Instance.Publish(new PoolerAddToTripMessage
                {
                    Trip = commuterRequestProcess.Trip,
                    Pooler = commuterRequestProcess.PoolerRequests[index].TripRequest.Commuter
                });
            }
            return acceptedPoolerRequests;
        }

        public List<TripRequest> GetPoolersToRequestForTrip(TripRequest request)
        {
            //todo: a lot of read and write with multiple lists, try using enum for status
            CommuterRequestProcessModel commuterRequestProcess;
            if (!_engineState.CommuterRequestProcessTable.TryGetValue(request.Commuter.CommuterId, out commuterRequestProcess))
            {
                return new List<TripRequest>();
            }

            var waitingPoolers = commuterRequestProcess.PoolerRequests
                .Where(x => x.Status == TripRequestStatus.Waiting).ToList();
            int numberOfPoolersToRequest =
                (request.Commuter.Vehicle.RemainingSeats > waitingPoolers.Count) ?
                waitingPoolers.Count : request.Commuter.Vehicle.RemainingSeats;

            var poolersToRequest = waitingPoolers.GetRange(0, numberOfPoolersToRequest);
            foreach (var poolRequest in poolersToRequest)
            {
                var tripInRequest = commuterRequestProcess.PoolerRequests
                    .First(x => x.TripRequest.Commuter.CommuterId == poolRequest.TripRequest.Commuter.CommuterId);
                tripInRequest.Status = TripRequestStatus.Requested;
                TripRequestManager.UpdateRequestStatus(poolRequest.TripRequest, RequestStatus.InProcess);
            }

            return poolersToRequest.Select(x => x.TripRequest).ToList();
        }

        // r
        public IList<Commuter> GetPoolersInTrip(TripRequest request)
        {
            var commuterRequestProcess = _engineState.CommuterRequestProcessTable[request.Commuter.CommuterId];

            return commuterRequestProcess.Trip.Passengers;
        }

        // r
        public Models.Route GetTripRoute(TripRequest request)
        {
            CommuterRequestProcessModel processModel;
            if (_engineState.CommuterRequestProcessTable.TryGetValue(request.Commuter.CommuterId, out processModel))
                return processModel.CompleteRoute;
            return null;
        }

        //r, w
        public MethodResponse CompleteCommuterRequest(TripRequest request)
        {
            var commuterId = request.Commuter.CommuterId;
            if (request.GoingHow == GoingHow.Drive)
            {
                //todo: if the result is false, store request for later processing and inform customer
                return _engineState.RemoveTripFromState(request.Commuter);
            }

            Guid tripOwnerId;
            if (!_engineState.PoolerCommuterMapping.TryGetValue(commuterId, out tripOwnerId))
            {
                return new MethodResponse { IsSuccess = false };
            }

            return _engineState.RemovePoolerRequestFromState(tripOwnerId, commuterId);
        }
    }
}
