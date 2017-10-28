﻿namespace Bot.MessagingFramework
{
    internal interface IMessageHandler
    {
        int MaxRetryCount { get; set; }
        int RetryCount { get; set; }

        void EnqueueMessage<T>(T message);
    }
}