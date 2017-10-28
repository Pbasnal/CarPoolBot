using Bot.Data;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class AddPoolersToTripMessage : MessageBase
    {
        public TripRequest TripRequest { get; set; }
        public int[] PoolerIndices { get; set; }
    }
}
