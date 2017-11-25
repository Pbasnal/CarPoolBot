using Bot.Worker;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class WorkerDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(StartTheBackgroundWorker);
            return Task.CompletedTask;
        }

        private async Task StartTheBackgroundWorker(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                await context.PostAsync("Starting the worker");
                //PoolingEngine.Instance.QueuePoolingRequest();
                await context.PostAsync("Started the worker");

                context.Wait(ReturnCurrentStateOfWorker);
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }

        private async Task ReturnCurrentStateOfWorker(IDialogContext context, IAwaitable<object> result)
        {
            //try
            //{
            //    var state = PoolingEngine.State;

            //    await context.PostAsync(" state : " + state.IsWorking);

            //    if (state.IsWorking)
            //        context.Wait(ReturnCurrentStateOfWorker);
            //    else
            //        context.Done(" worker completed");
            //}
            //catch (Exception ex)
            //{
            //    var s = ex.Message;
            //    context.Fail(ex);
            //}
        }
    }
}