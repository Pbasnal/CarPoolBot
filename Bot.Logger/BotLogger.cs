using Bot.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Logger
{
    public class BotLogger<T>
    {
        static readonly IDictionary<Guid, IList<LogObject>> _flowLogs;
        static Guid AppId;
        static string LogFilePath;
        LogObject LogObject;
        
        public string Message;

        public BotLogger(Guid operationId, Guid flowId, string eventCode, T payload)
        {
            Log(operationId, flowId, eventCode, payload.ToJsonString());
        }

        public BotLogger(Guid operationId, Guid flowId, string eventCode, T payload, Exception exception)
        {
            Log(operationId, flowId, eventCode, payload.ToJsonString(), exception);
        }

        static BotLogger()
        {
            _flowLogs = new Dictionary<Guid, IList<LogObject>>();
            var configAppId = ConfigurationManager.AppSettings["LogAppId"];
            if (string.IsNullOrWhiteSpace(configAppId))
            {
                throw new ArgumentException(LogErrorMessages.LogAppIdNotFound);
            }

            if (!Guid.TryParse(configAppId, out AppId))
            {
                throw new ArgumentException(LogErrorMessages.LogAppIdIsInvalid);
            }

            LogFilePath = ConfigurationManager.AppSettings["LogFilePath"];
            if (string.IsNullOrWhiteSpace(LogFilePath))
            {
                throw new ArgumentException(LogErrorMessages.LogFilePathNotFound);
            }
        }

        public void Debug()
        {
            LogObject.LogType = LogType.Debug;
            LogObject.Message = Message;
            WriteToPermanentStore();
        }

        public void Warning()
        {
            LogObject.LogType = LogType.Warning;
            LogObject.Message = Message;
            WriteToPermanentStore();
        }

        public void Error()
        {
            LogObject.LogType = LogType.Error;
            LogObject.Message = Message;
            WriteToPermanentStore();
        }

        public void Exception()
        {
            LogObject.LogType = LogType.Exception;
            LogObject.Message = Message;
            WriteToPermanentStore();
        }

        private void Log(Guid operationId, Guid flowId, string eventCode, string payload)
        {
            if (AppId == Guid.Empty)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(LogFilePath))
            {
                throw new ArgumentException(LogErrorMessages.LogFilePathNotFound);
            }

            LogObject = new LogObject
            {
                AppId = AppId,
                OperationId = operationId,
                FlowId = flowId,
                EventCode = eventCode,
                Payload = payload,
                ThreadId = Thread.CurrentThread.ManagedThreadId.ToString(),
                LogTime = DateTime.UtcNow
            };

            if (!_flowLogs.ContainsKey(flowId))
            {
                _flowLogs.Add(flowId, new List<LogObject> { LogObject });
            }
            else
            {
                _flowLogs[flowId].Add(LogObject);
            }
        }

        private void Log(Guid operationId, Guid flowId, string eventCode, string payload, Exception exception)
        {
            if (AppId == Guid.Empty)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(LogFilePath))
            {
                throw new ArgumentException(LogErrorMessages.LogFilePathNotFound);
            }

            LogObject = new ExceptionLogObject
            {
                AppId = AppId,
                OperationId = operationId,
                FlowId = flowId,
                EventCode = eventCode,
                Payload = payload,
                ThreadId = Thread.CurrentThread.ManagedThreadId.ToString(),
                Exception = exception,
                LogTime = DateTime.UtcNow
            };

            if (!_flowLogs.ContainsKey(flowId))
            {
                _flowLogs.Add(flowId, new List<LogObject> { LogObject });
            }
            else
            {
                _flowLogs[flowId].Add(LogObject);
            }
        }

        private async void WriteToPermanentStore()
        {
            await WriteToLogDb();
        }

        private Task WriteToLogDb()
        {

            var str = ConfigurationManager.ConnectionStrings;
            var dbTask = new TaskFactory().StartNew(() =>
            {
                using (var ctx = new LogDatabaseContext())
                {
                    if (LogObject.LogType == LogType.Exception)
                    {
                        ctx.ExceptionLogs.Add((ExceptionLogObject)LogObject);
                    }
                    else
                    {
                        ctx.Logs.Add(LogObject);
                    }
                    ctx.SaveChanges();
                }
            });

            return dbTask;
        }
    }
}

