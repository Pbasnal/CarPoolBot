using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Retry
{
    public class RetryObject
    {
        private Guid OperationId;
        private Guid FlowId;
        private Action RetryOperation = null;
        private Func<Exception, bool> ProcessException = null;
        public int RetryIntervalInMilliseconds = 100;
        public int MaxRetries = 3;

        public RetryObject(Guid operationId, Guid flowId, Action retryOperation, Func<Exception, bool> processException)
        {
            OperationId = operationId;
            FlowId = flowId;
            RetryOperation = retryOperation;
            ProcessException = processException;
        }

        public async void Execute()
        {
            await Task.Run(() => Do());
        }

        private void Do()
        {
            var currentRetryCount = 0;

            while (currentRetryCount < MaxRetries)
            {
                try
                {
                    RetryOperation();
                }
                catch (Exception ex)
                {
                    bool stopRetry = false;
                    if (ProcessException != null)
                    {
                        stopRetry = ProcessException(ex);
                    }

                    if (stopRetry)
                    {
                        throw ex;
                    }

                    Thread.Sleep(RetryIntervalInMilliseconds);
                }
                finally
                {
                    currentRetryCount++;
                }
            }
        }
    }
}
