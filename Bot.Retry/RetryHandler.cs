using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace Bot.Retry
{
    public class RetryHandler
    {
        static IList<RetryObject> RetryList;
        static Thread RetryThread;

        public void Do(Action action, int maxAttemptCount = 3)
        {
            int interval;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["DefaultRetryIntervalInMinutes"], out interval))
            {
                interval = 2;
            }

            RetryObject retryObject = new RetryObject
            {
                Action = action,
                MaxRetryAttempts = maxAttemptCount,
                RetryAttempts = 0,
                WaitTime = new TimeSpan(interval),
                SubmittedTime = DateTime.UtcNow,
                LastRetryAttmeptTime = DateTime.UtcNow
            };

            if (RetryList == null)
                RetryList = new List<RetryObject>();
            RetryList.Add(retryObject);

            if (RetryThread != null)
            {
                RetryThread = new Thread(new ThreadStart(RunAsync));
                RetryThread.IsBackground = true;
                RetryThread.Start();
            }
        }

        private void RunAsync()
        {
            TimeSpan minWaitTime = TimeSpan.MaxValue;
            List<RetryObject> retriesToRemove = new List<RetryObject>();

            while (RetryList.Count > 0)
            {
                minWaitTime = TimeSpan.MaxValue;
                retriesToRemove.Clear();

                foreach (var retryObject in RetryList)
                {
                    if (retryObject.RetryAttempts >= retryObject.MaxRetryAttempts)
                    {
                        retriesToRemove.Add(retryObject);
                        continue;
                    }
                    var remainingTime = retryObject.WaitTime - (DateTime.UtcNow - retryObject.LastRetryAttmeptTime);
                    if (minWaitTime > remainingTime)
                    {
                        minWaitTime = remainingTime;
                    }
                    if (remainingTime.Seconds <= 0)
                    {
                        retryObject.LastRetryAttmeptTime = DateTime.UtcNow;
                        try
                        {
                            retryObject.Action();
                        }
                        catch (Exception ex)
                        {
                            var s = ex.Message;
                        }
                    }
                }
                foreach (var retryObject in retriesToRemove)
                {
                    RetryList.Remove(retryObject);
                }
                Thread.Sleep(minWaitTime);
            }
        }
    }

    class RetryObject
    {
        protected internal Action Action;
        protected internal int MaxRetryAttempts;
        protected internal int RetryAttempts;
        protected internal TimeSpan WaitTime;
        protected internal DateTime SubmittedTime;
        protected internal DateTime LastRetryAttmeptTime;
    }
}
