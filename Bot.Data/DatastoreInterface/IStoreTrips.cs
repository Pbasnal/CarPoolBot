using Bot.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Data.DatastoreInterface
{
    public interface IStoreTrips
    {
        Task<bool> AddTripsAsync(IList<Trip> trips);

        Task<bool> UpdateTripsAsync(IList<Trip> trips);

        Task<List<Trip>> GetTrips(IList<Guid> tripIds);
    }
}
