using System;
using System.ComponentModel.DataAnnotations;

namespace Bot.Data.Models
{
    public class TripRequest : ModelBase
    {
        [Key]
        public Guid TripRequestId { get; set; }
        public Commuter Commuter { get; set; }
        public GoingTo GoingTo { get; set; }
        public DateTime RequestTime { get; set; }
        public TimeSpan WaitTime { get; set; }
        public GoingHow GoingHow { get; set; }
        public RequestStatus Status { get; set; }

        public TripRequest(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
            TripRequestId = Guid.NewGuid();
        }
    }
}
