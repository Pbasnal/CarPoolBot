using Bot.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Data.DatastoreInterface
{
    public interface IStoreTripRequests
    {
        Task<bool> AddTripRequestsAsync(List<TripRequest> tripRequests);
        Task<bool> UpdateTripRequestAsync(List<TripRequest> tripRequests);
        Task<List<TripRequest>> GetWaitingTripRequests();
    }
}
