using System;

namespace Bot.Logger
{
    public class LogObject
    {
        public Guid AppId { get; set; }
        public Guid OperationId { get; set; }
        public Guid FlowId { get; set; }
        public string EventCode { get; set; }
        public string Payload { get; set; }
        public LogType LogType { get; set; }
        public string Message { get; set; }

        public DateTime LogTime { get; set; }
    }
}
