using Bot.Data;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class PoolerAddedToStateMessage : MessageBase
    {
        public Commuter Pooler { get; set; }
        public TripRequest TripRequest { get; set; }
        public Commuter VehicleOwner { get; set; }
    }
}
