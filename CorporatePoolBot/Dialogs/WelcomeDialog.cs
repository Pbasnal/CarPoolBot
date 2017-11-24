using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Threading;
using Bot.External;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class WelcomeDialog : IDialog<object>
    {
        private string _welcomeNote = "Hey!! :)";
        private UserAuthentication userAuthenticator;

        public Task StartAsync(IDialogContext context)
        {
            userAuthenticator = new UserAuthentication();
            context.Wait(WelcomeCommuter);
            return Task.CompletedTask;
        }

        private async Task WelcomeCommuter(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                await context.PostAsync(_welcomeNote);

                
                var methodResponse = userAuthenticator.Authenticate(activity);
                if (!methodResponse.IsSuccess)
                {
                    //check user progress
                    await context.PostAsync(methodResponse.ResponseMessage);
                    await context.PostAsync("Starting onboarding process");
                    await context.Forward(new OnboardingDialog(), WelcomeCommuter, activity, CancellationToken.None);
                }
                await context.Forward(new RequestDialog(), AfterProcessingUserRequest, activity, CancellationToken.None);
            }
            catch (Exception ex)
            {
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