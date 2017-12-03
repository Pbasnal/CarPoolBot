using Bot.Data.Models;
using Bot.MessagingFramework;
using Bot.Worker.Models;
using System.Collections.Generic;
using System;

namespace Bot.Worker.Messages
{
    public class ReqestOwnerToAcceptPoolersMessage : MessageBase
    {
        public ReqestOwnerToAcceptPoolersMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public List<TripRequestInProcess> PoolersToRequestFor { get; set; }
        public TripRequest OwnerRequest { get; set; }
        public int Status { get; set; }
    }
}
