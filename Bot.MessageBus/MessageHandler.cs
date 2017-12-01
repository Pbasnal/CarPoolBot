using Bot.Extensions;
using Bot.Logger;
using Bot.MessagingFramework.Constants;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Bot.MessagingFramework
{
    public abstract class MessageHandler<T> : IMessageHandler where T : MessageBase
    {
        public int MaxRetryCount { get; set; }

        public int RetryCount { get; set; }

        private Queue<T> _messageQueue;

        private object _baton;
        private ManualResetEvent _mre;
        private bool _shouldWork = true;

        Guid OperationIdForHandlerOperation = Guid.NewGuid();
        Guid FlowIdForHandlerOperation = Guid.NewGuid();

        public MessageHandler(Guid operationId, Guid flowId)
        {
            MaxRetryCount = 3;
            RetryCount = 0;
            _messageQueue = new Queue<T>();
            _mre = new ManualResetEvent(false);
            _baton = new object();

            MessageBus.Instance.RegisterHandler(this, typeof(T).FullName, operationId, flowId);

            var runAsynchThread = new Thread(new ThreadStart(RunAsync));
            runAsynchThread.IsBackground = true;
            runAsynchThread.Start();
        }

        public void EnqueueMessage<T1>(T1 message)
        {
            if (!(message is T))
            {
                throw new ArgumentException("Type of the message is not supported", nameof(message));
            }

            lock (_baton)
            {
                _messageQueue.Enqueue(message as T);
                _mre.Set();
            }
        }

        private void RunAsync()
        {
            new BotLogger(OperationIdForHandlerOperation, FlowIdForHandlerOperation, EventCodes.StartedMessageHandlerJob, _messageQueue.ToJsonString())
                    .Debug();
            while (_shouldWork)
            {
                lock (_baton)
                {
                    if (_messageQueue.Count <= 0)
                    {
                        _mre.Reset();
                    }
                }
                _mre.WaitOne();
                if (!_shouldWork)
                {
                    break;
                }

                T message;
                lock (_baton)
                {
                    message = _messageQueue.Dequeue();
                    new BotLogger(message.OperationId, message.MessageId, EventCodes.HandlingMessageAsync, message.ToJsonString())
                    .Debug();
                }
                HandleWithRetry(message);
            }
        }

        private void HandleWithRetry(T message)
        {
            while (RetryCount <= MaxRetryCount)
            {
                try
                {
                    RetryCount++;
                    new BotLogger(message.OperationId, message.MessageId, EventCodes.CallingHandlerWithRetries, message.ToJsonString())
                    {
                        Message = "Retries: " + RetryCount + "  MaxRetryCount: " + MaxRetryCount
                    }.Debug();

                    Handle(message);
                    break;
                }
                catch (Exception ex)
                {
                    if (RetryCount == MaxRetryCount + 1)
                    {
                        Tuple<T, Exception> payload = new Tuple<T, Exception>(message, ex);
                        new BotLogger(message.OperationId, message.MessageId, EventCodes.ExceptionCameInHandler, payload.ToJsonString())
                        {
                            Message = "Retries: " + RetryCount + "  MaxRetryCount: " + MaxRetryCount
                        }.Debug();
                    }
                    
                }
            }
            RetryCount = 0;
        }

        public void StopWorking()
        {
            _mre.Set();
            new BotLogger(OperationIdForHandlerOperation, FlowIdForHandlerOperation, EventCodes.StopingMessageHandlerJob, _messageQueue.ToJsonString())
                    .Debug();
            _shouldWork = false;
        }

        public void StartWorking()
        {
            if (_shouldWork)
            {
                new BotLogger(OperationIdForHandlerOperation, FlowIdForHandlerOperation, EventCodes.DidntStartMessageHandlerJob, _messageQueue.ToJsonString())
                {
                    Message = "_shouldWork" + _shouldWork.ToJsonString()
                }.Debug();
                return;
            }
            new BotLogger(OperationIdForHandlerOperation, FlowIdForHandlerOperation, EventCodes.StartingMessageHandlerJob, _messageQueue.ToJsonString())
                    .Debug();
            var runAsynchThread = new Thread(new ThreadStart(RunAsync));
            runAsynchThread.Start();
        }

        public abstract void Handle(T message);
    }
}
