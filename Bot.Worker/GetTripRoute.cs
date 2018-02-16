using Bot.Common;
using Bot.Logger;
using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    public class GetTripRoute : MessageHandler<GetTripRouteMessage>
    {
        GetTripRouteCore _core = new GetTripRouteCore();

        public GetTripRoute()
        {
            _core = new GetTripRouteCore();
        }

        public override void Handle(GetTripRouteMessage message)
        {
            try
            {
                new BotLogger<GetTripRouteMessage>(message.OperationId, message.MessageId, EventCodes.HandleGetTripRouteMessageBegin, message)
                    .Debug();

                message.Callback(_core.GetTripRoute(message));

                new BotLogger<GetTripRouteMessage>(message.OperationId, message.MessageId, EventCodes.HandleGetTripRouteMessageEnd, message)
                    .Debug();
            }
            catch (Exception ex)
            {
                new BotLogger<GetTripRouteMessage>(message.OperationId, message.MessageId, EventCodes.HandleGetTripRouteMessageException, message, ex)
                    .Debug();
                throw;
            }
        }
    }
}
