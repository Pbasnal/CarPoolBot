using Bot.Extensions;
using Bot.Logger;
using Bot.MessagingFramework.Constants;
using System;
using System.Collections.Generic;

namespace Bot.MessagingFramework
{
    public class MessageBus
    {
        private IDictionary<string, IList<IMessageHandler>> messageHandlerTable;
        
        private static MessageBus _instance;

        public static MessageBus Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MessageBus();
                return _instance;
            }
        }

        private MessageBus()
        {
            messageHandlerTable = new Dictionary<string, IList<IMessageHandler>>();
        }

        public void RegisterHandler(IMessageHandler handler, string messageType, Guid operationId, Guid flowId)
        {
            new BotLogger<IMessageHandler>(operationId, flowId, EventCodes.RegisteringNewHandler, handler)
            {
                Message = messageType
            }.Debug();


            if (messageHandlerTable.ContainsKey(messageType))
                messageHandlerTable[messageType].Add(handler);
            else
                messageHandlerTable.Add(messageType, new List<IMessageHandler> { handler });
        }

        public void Publish(MessageBase message)
        {
            new BotLogger<MessageBase>(message.OperationId, message.MessageId, EventCodes.PublishingMessage, message)
            .Debug();

            var key = message.GetType().FullName;

            if (!messageHandlerTable.ContainsKey(key))
                return;

            foreach (var handler in messageHandlerTable[key])
            {
                handler.EnqueueMessage(message);
            }
        }
    }
}
