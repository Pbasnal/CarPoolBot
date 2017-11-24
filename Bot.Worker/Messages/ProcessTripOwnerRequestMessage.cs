using Bot.Data;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class ProcessTripOwnerRequestMessage : MessageBase
    {
        public TripRequest TripOwnerRequest { get; set; }
    }
}
