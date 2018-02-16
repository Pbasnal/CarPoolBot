using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bot.Extensions;

namespace Bot.Logger
{
    public class LogObject
    {
        [Key]
        public int LogId { get; set; }
        public Guid AppId { get; set; }
        public Guid OperationId { get; set; }
        public Guid FlowId { get; set; }
        public string EventCode { get; set; }
        public string Payload { get; set; }
        public LogType LogType { get; set; }
        public string Message { get; set; }
        public string ThreadId { get; set; }
        //public string MachineName { get; set; }
        // log the below as well
        //public SystemInformation SystemInformation {get;set;}

        public DateTime LogTime { get; set; }
    }

    public class ExceptionLogObject : LogObject
    {

        public string ExceptionString {
            get { return Exception.ToJsonString(); }
            set { }
        }

        [NotMapped]
        public Exception Exception { get; set; }
    }
}
