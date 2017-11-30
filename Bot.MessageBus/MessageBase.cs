using System;

namespace Bot.MessagingFramework
{
    public class MessageBase
    {
        Guid OperationId { get; set; }
        Guid MessageId { get; set; }

        public MessageBase()
        {
            MessageId = Guid.NewGuid();
        }
    }
}
