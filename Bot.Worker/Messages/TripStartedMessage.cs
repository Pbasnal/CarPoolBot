using Bot.Data;
using Bot.Data.Models;
using Bot.MessagingFramework;
using Bot.Worker.Models;

namespace Bot.Worker.Messages
{
    public class TripStartedMessage : MessageBase
    {
        public Trip Trip { get; set; }

        public Models.Route Route { get; set; }
    }
}
