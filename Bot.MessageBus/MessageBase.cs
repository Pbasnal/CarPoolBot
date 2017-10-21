using System;

namespace Bot.MessagingFramework
{
    public class MessageBase
    {
        Guid MessageId { get; set; }

        public MessageBase()
        {
            MessageId = Guid.NewGuid();
        }
    }
}
