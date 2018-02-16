using System;
using System.Collections.Generic;
using Bot.Data.DatastoreInterface;
using Bot.Data.EfDatastores;
using Bot.Data.EfModels;
using Bot.Data.Models;
using Bot.Models.Internal;

namespace Bot.Data.DataManagers
{
    public class TripManager
    {
        private static TripManager instance = null;

        IStoreTrips TripsStore;

        private TripManager()
        {
            TripsStore = new StoreTrips();
        }

        public static TripManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TripManager();
                return instance;
            }
            set
            { }
        }

        public IDictionary<Guid, Trip> CarPoolTrip = new Dictionary<Guid, Trip>();

        public MethodResponse<Trip> StartNewTrip(Guid flowId, Trip trip)
        {
            //var result = TripsStore.AddTripsAsync(trip.OperationId, flowId, new List<Trip> { trip }).Result;
            var result = true;
            if (result)
            {
                CarPoolTrip.Add(trip.Owner.CommuterId, trip);
                return new MethodResponse<Trip>(trip);
            }
            return new MethodResponse<Trip>(null);
        }
    }
}
