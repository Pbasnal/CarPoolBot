using System;

namespace Bot.MessagingFramework
{
    public class MessageBase
    {
        public Guid OperationId { get; set; }
        public Guid MessageId { get; set; }

        public MessageBase()
        {
            MessageId = Guid.NewGuid();
        }
    }
}
