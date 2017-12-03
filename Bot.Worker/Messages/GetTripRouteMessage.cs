using System;
using Bot.Data.Models;
using Bot.MessagingFramework;

namespace Bot.Worker.Messages
{
    public class GetTripRouteMessage : MessageBase
    {
        public GetTripRouteMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public delegate void GetTripRouteCallack(Models.Route route);

        public GetTripRouteCallack Callback { get; set; }

        public TripRequest TripRequest { get; set; }
    }
}
