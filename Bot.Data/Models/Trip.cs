using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bot.Data.Models
{
    public class Trip
    {
        [Key]
        public Guid TripId { get; set; }

        public GoingTo GoingTo { get; set; }
        public Commuter Owner { get; set; }
        public IList<Commuter> Passengers { get; set; }

        public Trip()
        {
            TripId = Guid.NewGuid();
        }

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
