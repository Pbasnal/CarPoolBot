using Bot.Data.Models;
using Bot.MessagingFramework;
using System;

namespace Bot.Worker.Messages
{
    public class TripDidNotStartedMessage : MessageBase
    {
        public TripDidNotStartedMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public Trip Trip { get; set; }

        public Models.Route Route { get; set; }
    }
}
