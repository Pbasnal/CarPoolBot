using Bot.MessagingFramework;
using System;

namespace Bot.Worker.Messages
{
    public class TripRequestFailedMessage : MessageBase
    {
        public MessageBase FailedMessage { get; set; }
        public IMessageHandler FailedHandler { get; set; }

        public TripRequestFailedMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }
    }
}
