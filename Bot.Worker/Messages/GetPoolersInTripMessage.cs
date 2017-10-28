using Bot.Data;
using Bot.MessagingFramework;
using System.Collections.Generic;

namespace Bot.Worker.Messages
{
    public class GetPoolersInTripMessage : MessageBase
    {
        public delegate void GetPoolersCallback(IList<Commuter> poolers);

        public GetPoolersCallback Callback { get; set; }

        public TripRequest TripRequest { get; set; }
    }
}
