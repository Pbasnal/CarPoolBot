using Bot.Data.Models;
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
        }

        public EfTripRequest(TripRequest tripRequest)
        {
            TripRequestId = tripRequest.TripRequestId;
            Commuter = (EfCommuter)tripRequest.Commuter;
            GoingTo = tripRequest.GoingTo;
            RequestTime = tripRequest.RequestTime;
            WaitTime = tripRequest.WaitTime;
            GoingHow = tripRequest.GoingHow;
            Status = tripRequest.Status;
        }

        public static explicit operator EfTripRequest(TripRequest tripRequest)
        {
            return new EfTripRequest(tripRequest);
        }

        public TripRequest GetTripRequest(Guid operationId, Guid flowId)
        {
            return new TripRequest(operationId)
            {
                TripRequestId = TripRequestId,
                Commuter = Commuter.GetCommuter(operationId, flowId),
                GoingHow = GoingHow,
                GoingTo = GoingTo,
                RequestTime = RequestTime,
                Status = Status,
                WaitTime = WaitTime
            };
        }
    }
}
