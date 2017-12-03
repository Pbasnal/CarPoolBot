using Bot.Common;
using Bot.Logger;
using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    public class EndTheTrip : MessageHandler<EndTripMessage>
    {
        private EndTheTripCore _core;

        public EndTheTrip()
        {
            _core = new EndTheTripCore();
        }

        public override void Handle(EndTripMessage message)
        {
            try
            {
                new BotLogger<EndTripMessage>(message.OperationId, message.MessageId, EventCodes.HandleEndTripMessageBegin, message)
                    .Debug();

                _core.CompleteCommuterRequest(message.TripRequest);

                new BotLogger<EndTripMessage>(message.OperationId, message.MessageId, EventCodes.HandleEndTripMessageEnd, message)
                    .Debug();
            }
            catch (Exception ex)
            {
                new BotLogger<EndTripMessage>(message.OperationId, message.MessageId, EventCodes.HandleEndTripMessageException, message, ex)
                    .Exception();
                throw;
            }
        }
    }
}
