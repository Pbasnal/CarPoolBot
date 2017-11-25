using System;
using System.ComponentModel.DataAnnotations;

namespace Bot.Data.EfModels
{
    public class EfTripRequest
    {
        [Key]
        public Guid TripRequestId { get; set; }
        public EfCommuter Commuter { get; set; }
        public GoingTo GoingTo { get; set; }
        public DateTime RequestTime { get; set; }
        public TimeSpan WaitTime { get; set; }
        public GoingHow GoingHow { get; set; }
        public RequestStatus Status { get; set; }

        public EfTripRequest()
        {
            TripRequestId = Guid.NewGuid();
        }
    }
}
