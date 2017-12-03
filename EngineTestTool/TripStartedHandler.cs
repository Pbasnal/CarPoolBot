using Bot.MessagingFramework;
using Bot.Worker.Messages;

namespace EngineTestTool
{
    public class TripStartedHandler : MessageHandler<TripStartedMessage>
    {
        static int TotalProcessed = 0;
        
        public override void Handle(TripStartedMessage message)
        {
            Program.Map.AddCommuterAndPassengersToList(message);
            Program.Map.UpdateState(TotalProcessed++);
            //Program.Map.DisplayTrip(message);
        }
    }
}
