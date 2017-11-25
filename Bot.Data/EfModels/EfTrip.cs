using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bot.Data.EfModels
{
    public class EfTrip
    {
        [Key]
        public Guid TripId { get; set; }
        public GoingTo GoingTo { get; set; }
        public EfCommuter Owner { get; set; }
        public IList<EfCommuter> Passengers { get; set; }
    }
}
