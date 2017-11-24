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

        public MessageHandler()
        {
            MaxRetryCount = 3;
            RetryCount = 0;
            _messageQueue = new Queue<T>();
            _mre = new ManualResetEvent(false);
            _baton = new object();

            MessageBus.Instance.RegisterHandler(this, typeof(T).FullName);

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
                }
                HandleWithRetry(message);
            }
        }

        private void HandleWithRetry(T message)
        {
            while (RetryCount <= MaxRetryCount)
            {
                RetryCount++;

                try
                {
                    Handle(message);
                    break;
                }
                catch (Exception ex)
                {
                    if (RetryCount == MaxRetryCount)
                    {
                        //log exception
                    }
                }
            }
            RetryCount = 0;
        }

        public void StopWorking()
        {
            _mre.Set();
            _shouldWork = false;
        }

        public void StartWorking()
        {
            if (_shouldWork)
            {
                return;
            }
            var runAsynchThread = new Thread(new ThreadStart(RunAsync));
            runAsynchThread.Start();
        }

        public abstract void Handle(T message);
    }
}
