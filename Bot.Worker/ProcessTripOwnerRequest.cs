using Bot.Common;
using Bot.Extensions;
using Bot.Logger;
using Bot.MessagingFramework;
using Bot.Models.Internal;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    //todo: handle callbacks 
    public class ProcessTripOwnerRequest : MessageHandler<ProcessTripOwnerRequestMessage>
    {
        private ProcessTripOwnerRequestCore _core;

        public ProcessTripOwnerRequest()
        {
            _core = new ProcessTripOwnerRequestCore();
        }

        public override void Handle(ProcessTripOwnerRequestMessage message)
        {
            try
            {
                new BotLogger<ProcessTripOwnerRequestMessage>(message.OperationId, message.MessageId, EventCodes.HandleProcessTripOwnerRequestMessageBegin, message)
                    .Debug();

                var methodResponse = _core.ProcessRequests(message);

                if (methodResponse.IsSuccess)
                {
                    new BotLogger<ProcessTripOwnerRequestMessage>(message.OperationId, message.MessageId, EventCodes.HandleProcessTripOwnerRequestMessageEnd, message)
                    .Debug();
                    return;
                }

                new BotLogger<ProcessTripOwnerRequestMessage>(message.OperationId, message.MessageId, EventCodes.HandleProcessTripOwnerRequestMessageFailed, message)
                {
                    Message = "MethodResponse : " + methodResponse.ToJsonString()
                }.Error();

                if (methodResponse.ResponseCode == ResponseCodes.InternalProcessErrorDoNotRetry)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                new BotLogger<ProcessTripOwnerRequestMessage>(message.OperationId, message.MessageId, EventCodes.HandleProcessTripOwnerRequestMessageException, message, ex)
                    .Exception();
                throw;
            }
        }
    }
}
