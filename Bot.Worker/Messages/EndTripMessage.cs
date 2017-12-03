using System;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class EndTripMessage : MessageBase
    {
        public EndTripMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public TripRequest TripRequest { get; set; }
    }
}
