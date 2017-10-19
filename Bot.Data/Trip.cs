using System.Collections.Generic;

namespace Bot.Data
{
    public class Trip
    {
        public GoingTo GoingTo;
        public Commuter Owner;
        public IList<Commuter> Passengers;

        public Coordinate Origin
        {
            get
            {
                if (GoingTo == GoingTo.Home)
                    return Owner.OfficeCoordinate;
                else
                    return Owner.HomeCoordinate;
            }
        }

        public bool AddPassenger(Commuter passenger)
        {
            var remainingSeats = Owner.Vehicle.MaxPassengerCount - Owner.Vehicle.OccupiedSeats;
            if (remainingSeats > 0)
            {
                Passengers.Add(passenger);
                Owner.Vehicle.OccupiedSeats++;
                return true;
            }
            return false;
        }
    }
}
