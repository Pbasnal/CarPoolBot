using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Common;
using Bot.Data.DatastoreInterface;
using Bot.Data.EfModels;
using Bot.Data.Models;
using Bot.Logger;

namespace Bot.Data.EfDatastores
{
    public class StoreTripRequests : IStoreTripRequests
    {
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
                    using (var dbCtx = new DatabaseContext())
                    {
                        dbCtx.EfTripRequests.AddRange(tripRequests.Select(x => (EfTripRequest)x));
                        dbCtx.SaveChanges(); 
                    }
                    new BotLogger<List<TripRequest>>(operationId, flowId, EventCodes.AddedTripRequestToEntityFrameworkSqlDb, tripRequests)
                        .Debug();
                }
                catch (Exception ex)
                {
                    new BotLogger<List<TripRequest>>(operationId, flowId, EventCodes.ExceptionWhileAddingTripToEntityFrameworkSqlDb, tripRequests, ex)
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
                    using (var dbCtx = new DatabaseContext())
                    {
                        var efTripRequestsToUpdate = dbCtx.EfTripRequests.Where(x => tripRequestIds.Contains(x.TripRequestId)).ToList();

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

        public async Task<List<TripRequest>> GetWaitingTripRequests()
        {
            var result = new TaskFactory().StartNew<List<TripRequest>>(() =>
            {

                using (var dbCtx = new DatabaseContext())
                {
                    var eftripRequests =
                        dbCtx.EfTripRequests.Where(x => x.Status == RequestStatus.Initialized || x.Status == RequestStatus.Waiting)
                            .ToList();

                    return eftripRequests.Select(x => x.GetTripRequest(Guid.NewGuid(), Guid.NewGuid())).ToList(); 
                }
            }).Result;
            return result;
        }
    }
}
