using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bot.MessagingFramework;
using System.Threading;
using System;

namespace MessagingBusUnitTests
{
    [TestClass]
    public class MessageBusUnitTests
    {
        [TestMethod]
        public void BasicTest()
        {
            var message = new TextMessage(Guid.NewGuid(), Guid.NewGuid());
            var message2 = new TextMessage(message.OperationId, message.MessageId);

            message.msg = "message aaya";
            message2.msg = "Message 2 aaya";

            var h1 = new MockMessageHandler();
            //var h2 = new MockMessageHandler();

            MessageBus.Instance.Publish(message);
            Thread.Sleep(1000);
            //MessageBus.Instance.Publish(message2);

            

            //Assert.AreEqual(message.msg, h1.Message);
            //Assert.AreEqual(message2.msg, h1.Message);
        }
    }

    public class TextMessage : MessageBase
    {
        public string msg;

        public TextMessage(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }
    }

    public class TextMessage2 : MessageBase
    {
        public string msg;

        public TextMessage2(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }
    }

    internal class MockMessageHandler2 : MessageHandler<TextMessage2>
    {
        public string Message;

        public override void Handle(TextMessage2 message)
        {
            Message = message.msg;
        }
    }

    internal class MockMessageHandler : MessageHandler<TextMessage>
    {
        public string Message;
        
        public override void Handle(TextMessage message)
        {
            Message = message.msg;
            throw new Exception();
        }
    }
}
