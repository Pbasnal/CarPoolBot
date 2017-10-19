using System;

namespace Bot.Data
{
    public class TripRequest
    {
        public Commuter Commuter;
        public GoingTo GoingTo;
        public DateTime RequestTime;
        public TimeSpan WaitTime;
        public GoingHow GoingHow;
        public RequestStatus Status;
    }
}
