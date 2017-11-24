using Bot.Data;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class PoolerAddToTripMessage : MessageBase
    {
        public Trip Trip { get; set; }
        public Commuter Pooler { get; set; }
    }
}
