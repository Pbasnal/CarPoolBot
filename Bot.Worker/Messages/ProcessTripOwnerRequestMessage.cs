using Bot.Data;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class ProcessTripOwnerRequestMessage : MessageBase
    {
        public Commuter Commuter { get; set; }
    }
}
