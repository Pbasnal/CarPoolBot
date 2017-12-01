using Bot.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Bot.Logger
{
    public class BotLogger
    {
        static IDictionary<Guid, IList<LogObject>> _flowLogs;
        static Guid AppId;
        static string LogFilePath;
        LogObject LogObject;

        public string Message;
        
        public BotLogger(Guid operationId, Guid flowId, string eventCode, string payload)
        {
            Log(operationId, flowId, eventCode, payload);
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
            using (StreamWriter outputFile = new StreamWriter(LogFilePath + LogObject.OperationId.ToString() + ".txt", true))
            {
                await outputFile.WriteLineAsync(LogObject.ToJsonString());
            }
        }
    }
}

