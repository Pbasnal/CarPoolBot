namespace Bot.MessagingFramework.Constants
{
    internal class EventCodes
    {
        public const string RegisteringNewHandler = "RegisteringNewHandler";
        public const string PublishingMessage = "PublishingMessage";
        public const string HandlingMessageAsync = "HandlingMessageAsync";
        public const string CallingHandlerWithRetries = "CallingHandlerWithRetries";
        public const string ExceptionCameInHandler = "ExceptionCameInHandler";
        public const string StartingMessageHandlerJob = "StartingMessageHandlerJob";
        public const string StopingMessageHandlerJob = "StopingMessageHandlerJob";
        public const string StartedMessageHandlerJob = "StartedMessageHandlerJob";
        public const string DidntStartMessageHandlerJob = "DidntStartMessageHandlerJob";
    }
}
