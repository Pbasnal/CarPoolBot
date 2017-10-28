using Bot.Data;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class GetTripRouteMessage : MessageBase
    {
        public delegate void GetTripRouteCallack(Models.Route route);

        public GetTripRouteCallack Callback { get; set; }

        public TripRequest TripRequest { get; set; }
    }
}
