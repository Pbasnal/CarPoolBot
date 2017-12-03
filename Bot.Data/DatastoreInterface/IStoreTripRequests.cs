using Bot.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Data.DatastoreInterface
{
    public interface IStoreTripRequests
    {
        Task<bool> AddTripRequestsAsync(Guid operationId, Guid flowId, List<TripRequest> tripRequests);
        Task<bool> UpdateTripRequestAsync(List<TripRequest> tripRequests);
        Task<List<TripRequest>> GetWaitingTripRequests();
    }
}
