using System;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class PoolerAddedToTripMessage : MessageBase
    {
        public PoolerAddedToTripMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public Trip Trip { get; set; }
        public Commuter Pooler { get; set; }
    }
}
