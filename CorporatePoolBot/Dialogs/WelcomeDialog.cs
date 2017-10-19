using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Threading;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class WelcomeDialog : IDialog<object>
    {
        private string _welcomeNote = "Hey!! :)";
        
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(WelcomeCommuter);
            return Task.CompletedTask;
        }

        private async Task WelcomeCommuter(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                await context.PostAsync(_welcomeNote);
                //Check if user is onboarded
                await context.Forward(new WorkerDialog(), AfterProcessingUserRequest, activity, CancellationToken.None);
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }

        private async Task AfterProcessingUserRequest(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var msg = await result as string;
                await context.PostAsync(msg);
                context.Wait(WelcomeCommuter);
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }
    }
}