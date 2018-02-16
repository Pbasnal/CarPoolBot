﻿using Bot.Data.Models;
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

        public EfTrip()
        {
            
        }

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

        public Trip GetTrip(Guid operationId, Guid flowId)
        {
            return new Trip(TripId)
            {
                GoingTo = GoingTo,
                Owner = Owner.GetCommuter(operationId, flowId),
                TripId = TripId,
                Passengers = Passengers.Select(x => x.GetCommuter(operationId, flowId)).ToList()
            };
        }

    }

    
}
