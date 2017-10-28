using Bot.Data;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class TripStartedMessage : MessageBase
    {
        public Trip Trip { get; set; }
    }
}
