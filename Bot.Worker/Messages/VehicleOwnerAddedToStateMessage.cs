using Bot.Data;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class VehicleOwnerAddedToStateMessage : MessageBase
    {
        public Commuter VehicleOwner { get; set; }
        
        public TripRequest TripRequest { get; set; }
    }
}
