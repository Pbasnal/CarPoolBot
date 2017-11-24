using Bot.Data;
using Bot.Data.Models;
using Bot.MessagingFramework;
using Bot.Worker.Models;
using System.Collections.Generic;

namespace Bot.Worker.Messages
{
    public class ReqestOwnerToAcceptPoolersMessage : MessageBase
    {
        public List<TripRequestInProcess> PoolersToRequestFor { get; set; }
        public TripRequest OwnerRequest { get; set; }
        public int Status { get; set; }
    }
}
