using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bot.Data.EfModels;

namespace Bot.Data.Models
{
    public class Trip : ModelBase
    {
        [Key]
        public Guid TripId { get; set; }

        public GoingTo GoingTo { get; set; }
        public Commuter Owner { get; set; }
        public TripRequest OwnerRequest { get; set; }
        public IList<Commuter> Passengers { get; set; }
        public IList<TripRequest> TripRequests { get; set; }


        public Trip(Guid operationId) : base(operationId)
        {
            TripId = operationId;
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
