using Bot.Common;
using Bot.Data.DatastoreInterface;
using Bot.Data.Models;
using Bot.Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Data.EfModels
{
    public class DatabaseContext : DbContext, IStoreTrips, IStoreCommuters,
        IStoreTripRequests
    {
        public DatabaseContext() : base("name=ConnectionString")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DatabaseContext>());
            Database.CommandTimeout = 300;
        }

        public DbSet<EfCommuter> EfCommuters { get; set; }
        public DbSet<EfTrip> EfTrips { get; set; }
        public DbSet<EfTripRequest> EfTripRequests { get; set; }
        public DbSet<EfVehicle> EfVehicles { get; set; }

        public async Task<bool> AddTripsAsync(IList<Trip> trips)
        {
            if (trips == null || trips.Count == 0)
                return false;

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    EfTrips.AddRange(trips.Select(x => (EfTrip)x));
                    SaveChanges();
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

        public async Task<bool> UpdateTripsAsync(IList<Trip> trips)
        {
            if (trips == null || trips.Count == 0)
                return false;

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    var EfTripsToUpdate = EfTrips.Where(x => trips.Any(y => x.TripId == y.TripId)).ToList();

                    if (EfTripsToUpdate.Count != trips.Count)
                        return false;

                    foreach (var eftrip in EfTripsToUpdate)
                    {
                        var trip = trips.FirstOrDefault(x => x.TripId == eftrip.TripId);
                        if (trip == null)
                            return false;
                        eftrip.Passengers = trip.Passengers.Select(x => (EfCommuter)x).ToList();
                    }

                    SaveChanges();
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

        public async Task<bool> AddCommutersAsync(Guid operationId, Guid flowId, IList<Commuter> commuters)
        {
            if (commuters == null || commuters.Count == 0)
                return false;

            new BotLogger<IList<Commuter>>(operationId, flowId, EventCodes.AddingCommutersToEntityFrameworkSqlDb, commuters)
                .Debug();

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    EfCommuters.AddRange(commuters.Select(x => (EfCommuter)x));
                    SaveChanges();
                }
                catch (Exception ex)
                {
                    new BotLogger<IList<Commuter>>(operationId, flowId,
                        EventCodes.ExceptionWhileAddingCommutersToEntityFrameworkSqlDb,
                        commuters, ex)
                    .Exception();
                    return false;
                }
                return true;
            }).Result;
            new BotLogger<IList<Commuter>>(operationId, flowId, EventCodes.CommutersAddedToEntityFrameworkSqlDb, commuters)
                .Debug();
            return result;
        }

        public async Task<bool> UpdateCommutersAsync(Guid operationId, Guid flowId, IList<Commuter> commuters)
        {
            if (commuters == null || commuters.Count == 0)
            {
                new BotLogger<string>(operationId, flowId, EventCodes.InvalidArguments, string.Empty)
                {
                    Message = "List of commuters is " + (commuters == null ? "null" : "empty")
                }.Error();
                return false;
            }

            new BotLogger<IList<Commuter>>(operationId, flowId, EventCodes.UpdatingCommutersToEntityFrameworkSqlDb, commuters)
                .Debug();

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    var EfCommutersToUpdate = EfCommuters.Where(x => commuters.Any(y => x.CommuterId == y.CommuterId)).ToList();

                    if (EfCommutersToUpdate.Count != commuters.Count)
                    {
                        Tuple<IList<EfCommuter>, IList<Commuter>> commutersLogObject = new Tuple<IList<EfCommuter>, IList<Commuter>>(EfCommutersToUpdate, commuters);
                        new BotLogger<Tuple<IList<EfCommuter>, IList<Commuter>>>(operationId, flowId, EventCodes.OneOrMoreCommuterDoesNotExist, commutersLogObject)
                        {
                            Message = "order of log object is EfCommuters : Commuters"
                        }.Error();
                        return false;
                    }

                    foreach (var efCommuter in EfCommutersToUpdate)
                    {
                        var commuter = commuters.FirstOrDefault(x => x.CommuterId == efCommuter.CommuterId);
                        if (commuter == null)
                            return false;
                        efCommuter.CommuterName = commuter.CommuterName;
                        efCommuter.HomeCoordinate = (EfCoordinate)commuter.HomeCoordinate;
                        efCommuter.OfficeCoordinate = (EfCoordinate)commuter.OfficeCoordinate;
                        efCommuter.MediaId = commuter.MediaId;
                        efCommuter.Status = commuter.Status;
                        efCommuter.Vehicle = (EfVehicle)commuter.Vehicle;
                    }

                    SaveChanges();
                    new BotLogger<IList<EfCommuter>>(operationId, flowId, EventCodes.CommutersUpdatedToEntityFrameworkSqlDb, EfCommutersToUpdate)
                        .Debug();
                    return true;
                }
                catch (Exception ex)
                {
                    new BotLogger<IList<Commuter>>(operationId, flowId, EventCodes.ExceptionWhileUpdatingCommutersToEntityFrameworkSqlDb, commuters, ex)
                        .Exception();
                    return false;
                }
            }).Result;

            return result;
        }

        public async Task<List<Trip>> GetTrips(IList<Guid> tripIds)
        {
            if (tripIds == null || tripIds.Count == 0)
                return new List<Trip>();

            var result = new TaskFactory().StartNew<List<Trip>>(() =>
            {
                var eftrips = EfTrips.Where(x => tripIds.Contains(x.TripId)).ToList();

                return eftrips.Select(x => x.GetTrip(Guid.NewGuid(), Guid.NewGuid())).ToList();
            }).Result;
            return result;
        }

        public async Task<List<Commuter>> GetCommuters(IList<Guid> commuterIds)
        {
            if (commuterIds == null || commuterIds.Count == 0)
                return new List<Commuter>();

            var result = new TaskFactory().StartNew<List<Commuter>>(() =>
            {
                var eftrips = EfCommuters.Where(x => commuterIds.Contains(x.CommuterId)).ToList();

                return eftrips.Select(x => x.GetCommuter(Guid.NewGuid(), Guid.NewGuid())).ToList();
            }).Result;
            return result;
        }

        public async Task<List<Commuter>> GetCommutersForMediaIds(List<string> mediaIds)
        {
            if (mediaIds == null || mediaIds.Count == 0)
                return new List<Commuter>();

            var result = new TaskFactory().StartNew<List<Commuter>>(() =>
            {
                var eftrips = EfCommuters.Where(x => mediaIds.Contains(x.MediaId)).ToList();

                return eftrips.Select(x => x.GetCommuter(Guid.NewGuid(), Guid.NewGuid())).ToList();
            }).Result;
            return result;
        }

        public async Task<bool> AddTripRequestsAsync(Guid operationId, Guid flowId, List<TripRequest> tripRequests)
        {
            new BotLogger<List<TripRequest>>(operationId, flowId, EventCodes.AddingTripRequestToEntityFrameworkSqlDb, tripRequests)
                .Debug();

            if (tripRequests == null || tripRequests.Count == 0)
            {
                new BotLogger<List<TripRequest>>(operationId, flowId, EventCodes.InvalidArguments, tripRequests)
                {
                    Message = "No trips provided"
                }.Error();
                return false;
            }

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    EfTripRequests.AddRange(tripRequests.Select(x => (EfTripRequest)x));
                    SaveChanges();
                    new BotLogger<List<TripRequest>>(operationId, flowId, EventCodes.AddedTripRequestToEntityFrameworkSqlDb, tripRequests)
                        .Debug();
                }
                catch (Exception ex)
                {
                    new BotLogger<List<TripRequest>>(operationId, flowId, EventCodes.AddedTripRequestToEntityFrameworkSqlDb, tripRequests, ex)
                        .Exception();
                    return false;
                }
                return true;
            }).Result;

            return result;
        }

        public async Task<bool> UpdateTripRequestAsync(List<TripRequest> tripRequests)
        {
            if (tripRequests == null || tripRequests.Count == 0)
                return false;

            var result = new TaskFactory().StartNew<bool>(() =>
            {
                try
                {
                    var tripRequestIds = tripRequests.Select(x => x.TripRequestId).ToList();
                    var efTripRequestsToUpdate = EfTripRequests.Where(x => tripRequestIds.Contains(x.TripRequestId)).ToList();

                    if (efTripRequestsToUpdate.Count != tripRequests.Count)
                        return false;

                    foreach (var efTripRequest in efTripRequestsToUpdate)
                    {
                        var tripRequest = tripRequests.FirstOrDefault(x => x.TripRequestId == efTripRequest.TripRequestId);
                        if (tripRequest == null)
                            return false;
                        efTripRequest.Status = tripRequest.Status;
                        efTripRequest.WaitTime = tripRequest.WaitTime;
                    }

                    SaveChanges();
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

        public async Task<List<TripRequest>> GetWaitingTripRequests()
        {
            var result = new TaskFactory().StartNew<List<TripRequest>>(() =>
            {
                var eftripRequests =
                EfTripRequests.Where(x => x.Status == RequestStatus.Initialized || x.Status == RequestStatus.Waiting)
                .ToList();

                return eftripRequests.Select(x => x.GetTripRequest(Guid.NewGuid(), Guid.NewGuid())).ToList();
            }).Result;
            return result;
        }
    }
}
