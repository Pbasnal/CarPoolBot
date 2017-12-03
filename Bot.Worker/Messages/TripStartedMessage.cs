using System;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class TripStartedMessage : MessageBase
    {
        public TripStartedMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public Trip Trip { get; set; }

        public Models.Route Route { get; set; }
    }
}
