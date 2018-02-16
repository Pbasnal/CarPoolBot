using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.Data.Models;

namespace Bot.Data.DatastoreInterface
{
    public interface IStoreTrips
    {
        Task<bool> AddTripsAsync(Guid operationId, Guid flowId, IList<Trip> trips);

        Task<bool> UpdateTripsAsync(IList<Trip> trips);

        Task<List<Trip>> GetTrips(IList<Guid> tripIds);
    }
}
