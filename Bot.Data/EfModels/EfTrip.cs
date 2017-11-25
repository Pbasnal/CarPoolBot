using Bot.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Bot.Data.EfModels
{
    public class EfTrip
    {
        [Key]
        public Guid TripId { get; set; }
        public GoingTo GoingTo { get; set; }
        public EfCommuter Owner { get; set; }
        public IList<EfCommuter> Passengers { get; set; }

        public EfTrip(Trip trip)
        {
            TripId = trip.TripId;
            GoingTo = trip.GoingTo;
            Owner = (EfCommuter)trip.Owner;
            Passengers = trip.Passengers.Select(x => (EfCommuter)x).ToList();
        }

        public static explicit operator EfTrip(Trip trip)
        {
            return new EfTrip(trip);
        }

        public Trip GetTrip()
        {
            return new Trip
            {
                GoingTo = GoingTo,
                Owner = Owner.GetCommuter(),
                TripId = TripId,
                Passengers = Passengers.Select(x => x.GetCommuter()).ToList()
            };
        }

    }

    
}
