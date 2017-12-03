using System;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class ProcessTripOwnerRequestMessage : MessageBase
    {
        public ProcessTripOwnerRequestMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public TripRequest TripOwnerRequest { get; set; }
    }
}
