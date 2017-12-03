using Bot.Common;
using Bot.Logger;
using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    public class AddPoolersToTrip : MessageHandler<AddPoolersToTripMessage>
    {
        private AddPoolersToTripCore _core;

        public AddPoolersToTrip()
        {
            _core = new AddPoolersToTripCore();
        }

        public override void Handle(AddPoolersToTripMessage message)
        {
            try
            {
                new BotLogger<AddPoolersToTripMessage>(message.OperationId, message.MessageId, EventCodes.HandleAddPoolersToTripMessageBegin, message)
                    .Debug();

                _core.AddPoolersToTrip(message);

                new BotLogger<AddPoolersToTripMessage>(message.OperationId, message.MessageId, EventCodes.HandleAddPoolersToTripMessageEnd, message)
                    .Debug();
            }
            catch (Exception ex)
            {
                new BotLogger<AddPoolersToTripMessage>(message.OperationId, message.MessageId, EventCodes.HandleAddPoolersToTripMessageException, message, ex)
                    .Exception();
                throw;
            }
        }
        
    }
}
