using Bot.Common;
using Bot.Logger;
using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    public class GetPoolersInTrip : MessageHandler<GetPoolersInTripMessage>
    {
        GetPoolersInTripCore _core;

        public GetPoolersInTrip()
        {
            _core = new GetPoolersInTripCore();
        }

        public override void Handle(GetPoolersInTripMessage message)
        {
            try
            {
                new BotLogger<GetPoolersInTripMessage>(message.OperationId, message.MessageId, EventCodes.HandleGetPoolersInTripMessageBegin, message)
                    .Debug();

                message.Callback(_core.GetPoolersInTrip(message.TripRequest));

                new BotLogger<GetPoolersInTripMessage>(message.OperationId, message.MessageId, EventCodes.HandleGetPoolersInTripMessageEnd, message)
                    .Debug();
            }
            catch (Exception ex)
            {
                new BotLogger<GetPoolersInTripMessage>(message.OperationId, message.MessageId, EventCodes.HandleGetPoolersInTripMessageException, message, ex)
                    .Exception();
                throw;
            }

        }
    }
}
