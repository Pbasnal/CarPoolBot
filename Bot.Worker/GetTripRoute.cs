using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    public class GetTripRoute : MessageHandler<GetTripRouteMessage>
    {
        EngineCoreSingleRequest _core = new EngineCoreSingleRequest();

        public GetTripRoute(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
            _core = new EngineCoreSingleRequest();
        }

        public override void Handle(GetTripRouteMessage message)
        {
            try
            {
                message.Callback(_core.GetTripRoute(message.TripRequest));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
