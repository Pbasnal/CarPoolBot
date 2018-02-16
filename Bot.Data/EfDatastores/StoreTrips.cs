using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Common;
using Bot.Data.DatastoreInterface;
using Bot.Data.EfModels;
using Bot.Data.Models;
using Bot.Extensions;
using Bot.Logger;

namespace Bot.Data.EfDatastores
{
    public class StoreTrips : IStoreTrips
    {
        public async Task<bool> AddTripsAsync(Guid operationId, Guid flowId, IList<Trip> trips)
        {
            if (trips == null || trips.Count == 0)
                return false;

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    var efTrips = trips.Select(x => (EfTrip)x).ToList();
                    if (efTrips.Count == 0)
                    {
                        return false;
                    }

                    var commuterIds = new List<Guid>();
                    var ownerIds = new List<Guid>();
                    var passengerIds = new List<Guid>();
                   
                    efTrips.ForEach(trip =>
                    {
                        ownerIds.Add(trip.Owner.CommuterId);
                        passengerIds.AddRange(trip.Passengers.Select(p => p.CommuterId));
                    });
                    commuterIds.AddRange(ownerIds);
                    commuterIds.AddRange(passengerIds);

                    new BotLogger<string>(operationId, flowId, EventCodes.AddingTripSqlDb, LogHelper.CreatePayload(trips, efTrips))
                    .Debug();

                    using (var dbCtx = new DatabaseContext())
                    {
                        foreach (var efTrip in efTrips)
                        {
                            var efCommuters = dbCtx.EfCommuters.Where(ec => commuterIds.Any(c => c == ec.CommuterId)).ToList();
                            efTrip.Owner = efCommuters.First(ec => efTrip.Owner.CommuterId == ec.CommuterId);
                            efTrip.Passengers = efCommuters.FindAll(ec => passengerIds.Any(p => p == ec.CommuterId));

                            dbCtx.EfTrips.Add(efTrip);
                        }
                        dbCtx.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    new BotLogger<IList<Trip>>(operationId, flowId, EventCodes.ExceptionWhileAddingTripToEntityFrameworkSqlDb, trips, ex)
                        .Exception();
                    return false;
                }
                return true;
            }).Result;

            return result;
        }

        public async Task<bool> UpdateTripsAsync(IList<Trip> trips)
        {
            if (trips == null || trips.Count == 0)
                return false;

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    using (var dbCtx = new DatabaseContext())
                    {
                        var EfTripsToUpdate = dbCtx.EfTrips.Where(x => trips.Any(y => x.TripId == y.TripId)).ToList();

                        if (EfTripsToUpdate.Count != trips.Count)
                            return false;

                        foreach (var eftrip in EfTripsToUpdate)
                        {
                            var trip = trips.FirstOrDefault(x => x.TripId == eftrip.TripId);
                            if (trip == null)
                                return false;
                            eftrip.Passengers = trip.Passengers.Select(x => (EfCommuter)x).ToList();
                        }

                        dbCtx.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    var s = ex.Message;
                    return false;
                }
                return true;
            }).Result;

            return result;
        }

        public async Task<List<Trip>> GetTrips(IList<Guid> tripIds)
        {
            if (tripIds == null || tripIds.Count == 0)
                return new List<Trip>();

            var result = new TaskFactory().StartNew<List<Trip>>(() =>
            {
                using (var dbCtx = new DatabaseContext())
                {
                    var eftrips = dbCtx.EfTrips.Where(x => tripIds.Contains(x.TripId)).ToList();

                    return eftrips.Select(x => x.GetTrip(Guid.NewGuid(), Guid.NewGuid())).ToList();
                }
            }).Result;
            return result;
        }
    }
}
