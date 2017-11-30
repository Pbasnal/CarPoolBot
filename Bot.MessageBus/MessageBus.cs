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

        private Queue<MessageBase> RetryQueue;

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
            new BotLogger(operationId, flowId, EventCodes.RegisteringNewHandler, handler.ToJsonString())
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
            var key = message.GetType().FullName;

            if (!messageHandlerTable.ContainsKey(key))
                return;

            foreach (var handler in messageHandlerTable[key])
            {
                handler.EnqueueMessage(message);
            }
        }

        public void AddToRetryQueue(MessageBase message)
        {

        }
    }
}
