using Bot.Data.Models;
using Bot.MessagingFramework;
using System.Collections.Generic;
using System;

namespace Bot.Worker.Messages
{
    public class GetPoolersInTripMessage : MessageBase
    {
        public GetPoolersInTripMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public delegate void GetPoolersCallback(IList<Commuter> poolers);

        public GetPoolersCallback Callback { get; set; }

        public TripRequest TripRequest { get; set; }
    }
}
