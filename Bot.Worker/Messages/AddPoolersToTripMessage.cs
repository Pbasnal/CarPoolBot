using Bot.Data;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class AddPoolersToTripMessage : MessageBase
    {
        public TripRequest TripRequest { get; set; }
        public int[] PoolerIndices { get; set; }
        public bool SearchForMorePoolers { get; set; }
    }
}
