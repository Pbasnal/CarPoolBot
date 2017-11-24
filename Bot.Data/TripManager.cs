using Bot.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Data
{
    public class TripManager
    {
        private static TripManager tripsList = null;

        private TripManager()
        { }

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

        public Trip StartNewTrip(Commuter owner)
        {
            if (owner.Vehicle.MaxPassengerCount - owner.Vehicle.OccupiedSeats == 0)
                return null;
            var trip = new Trip();
            trip.Owner = owner;
            trip.Passengers = new List<Commuter>();
            owner.Vehicle.OccupiedSeats++;

            CarPoolTrip.Add(owner.CommuterId, trip);

            return trip;
        }

        // future scope: adding multiple passengers
        public bool AddCommuterToTrip(Commuter owner, Commuter passenger)
        {
            Trip trip;
            if (!CarPoolTrip.TryGetValue(owner.CommuterId, out trip))
                return false;

            if (trip.Passengers.Count >= owner.Vehicle.MaxPassengerCount)
                return false;

            trip.Passengers.Add(passenger);
            return true;
        }
    }
}
