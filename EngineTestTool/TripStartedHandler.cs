using Bot.MessagingFramework;
using Bot.Worker.Messages;

namespace EngineTestTool
{
    public class TripStartedHandler : MessageHandler<TripStartedMessage>
    {
        public override void Handle(TripStartedMessage message)
        {
            Program.Map.AddCommuterAndPassengersToList(message);
            //Program.Map.DisplayTrip(message);
        }
    }
}
