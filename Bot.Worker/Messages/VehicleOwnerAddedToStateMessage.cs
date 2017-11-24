using Bot.Data;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class VehicleOwnerAddedToStateMessage : MessageBase
    {
        public Route Route { get; set; }

        public TripRequest TripRequest { get; set; }
    }
}
