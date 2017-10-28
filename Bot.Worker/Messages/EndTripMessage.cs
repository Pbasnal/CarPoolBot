using Bot.Data;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class EndTripMessage : MessageBase
    {
        public TripRequest TripRequest { get; set; }
    }
}
