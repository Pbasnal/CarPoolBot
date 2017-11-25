using Bot.Data.DatastoreInterface;
using Bot.Data.EfModels;
using Bot.Data.Models;
using Bot.Models.Internal;
using System;
using System.Collections.Generic;

namespace Bot.Data
{
    public class TripManager
    {
        private static TripManager tripsList = null;

        IStoreTrips TripsStore;

        private TripManager()
        {
            TripsStore = new DatabaseContext();
        }

        public static TripManager TripsList
        {
            get
            {
                if (tripsList == null)
                    tripsList = new TripManager();
                return tripsList;
            }
            set
            { }
        }

        public IDictionary<Guid, Trip> CarPoolTrip = new Dictionary<Guid, Trip>();

        public MethodResponse<Trip> StartNewTrip(Commuter owner)
        {
            if (owner.Vehicle.MaxPassengerCount - owner.Vehicle.OccupiedSeats == 0)
                return new MethodResponse<Trip>(null);

            var trip = new Trip();
            trip.Owner = owner;
            trip.Passengers = new List<Commuter>();
            owner.Vehicle.OccupiedSeats++;

            var result = TripsStore.AddTripsAsync(new List<Trip> { trip }).Result;

            if (result)
            {
                CarPoolTrip.Add(owner.CommuterId, trip);
                return new MethodResponse<Trip>(trip);
            }
            return new MethodResponse<Trip>(null);
        }

        // future scope: adding multiple passengers
        public MethodResponse<Trip> AddPassengerToTrip(Commuter owner, Commuter passenger)
        {
            Trip trip;
            if (!CarPoolTrip.TryGetValue(owner.CommuterId, out trip))
                return new MethodResponse<Trip>(null); 

            if (trip.Passengers.Count >= owner.Vehicle.MaxPassengerCount)
                return new MethodResponse<Trip>(null); ;

            if (TripsStore.UpdateTripsAsync(new List<Trip> { trip }).Result)
            {
                trip.Passengers.Add(passenger);
                return new MethodResponse<Trip>(trip);
            }

            return new MethodResponse<Trip>(null);
        }
    }
}
