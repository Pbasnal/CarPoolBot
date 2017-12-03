using System;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class PoolerAddedToStateMessage : MessageBase
    {
        public PoolerAddedToStateMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public Commuter Pooler { get; set; }
        public TripRequest TripRequest { get; set; }
        public Commuter VehicleOwner { get; set; }
    }
}
