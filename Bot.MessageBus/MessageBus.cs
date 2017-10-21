using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public void RegisterHandler(IMessageHandler handler, string messageType)
        {
            if (messageHandlerTable.ContainsKey(messageType))
                messageHandlerTable[messageType].Add(handler);
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
    }
}
