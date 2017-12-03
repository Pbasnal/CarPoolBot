using Bot.MessagingFramework;
using Bot.Worker.Messages;

namespace CorporatePoolBot.MessageHandlers
{
    public class TripStartedHandler : MessageHandler<TripStartedMessage>
    {
        static int TotalProcessed = 0;
        
        public override void Handle(TripStartedMessage message)
        {
            
        }
    }
}
