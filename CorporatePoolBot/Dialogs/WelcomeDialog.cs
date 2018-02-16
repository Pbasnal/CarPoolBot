using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Threading;
using Bot.External;
using Bot.Logger;
using Bot.Common;
using Bot.Models.Internal;
using AuthBot.Dialogs;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class WelcomeDialog : IDialog<object>
    {
        private string _welcomeNote = "Hey!! :)";
        private UserAuthentication userAuthenticator;

        private Guid OperationId { get; set; }
        private Guid WelcomeFlowId { get; set; }

        public WelcomeDialog(Guid operationId, Guid flowId)
        {
            OperationId = operationId;
            WelcomeFlowId = Guid.NewGuid();
        }

        public Task StartAsync(IDialogContext context)
        {
            userAuthenticator = new UserAuthentication(OperationId, WelcomeFlowId);
            context.Wait(WelcomeCommuter);
            return Task.CompletedTask;
        }

        private async Task WelcomeCommuter(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                new BotLogger<Activity>(OperationId, WelcomeFlowId, EventCodes.WelcomeDialogInitiatedForUser, activity)
                    .Debug();
                
                await context.PostAsync(_welcomeNote);

                //await context.Forward(new AuthenticationDialog(), WelcomeCommuter, activity, CancellationToken.None);
                //context.Done(result);
                var methodResponse = userAuthenticator.Authenticate(activity);
                if (!methodResponse.IsSuccess)
                {
                    await context.Forward(new AuthenticationDialog(), WelcomeCommuter, result, CancellationToken.None);
                    new BotLogger<MethodResponse>(OperationId, WelcomeFlowId, EventCodes.UserNotYetOnboarded, methodResponse)
                    .Debug();

                    await context.PostAsync(methodResponse.ResponseMessage);
                    await context.PostAsync("Starting onboarding process");
                    //await context.Forward(new OnboardingDialog(OperationId, WelcomeFlowId), WelcomeCommuter, activity, CancellationToken.None);
                    context.Done("sf");
                }

                new BotLogger<MethodResponse>(OperationId, WelcomeFlowId, EventCodes.StartingUserRequest, methodResponse)
                    .Debug();
                //check user progress
                await context.Forward(new RequestDialog(OperationId, WelcomeFlowId), AfterProcessingUserRequest, activity, CancellationToken.None);
            }
            catch (Exception ex)
            {
                new BotLogger<object>(OperationId, WelcomeFlowId, EventCodes.ExceptionWhileWelcomingUser, result, ex)
                    .Exception();

                context.Fail(ex);
            }
        }

        private async Task AfterProcessingUserRequest(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                new BotLogger<object>(OperationId, WelcomeFlowId, EventCodes.UserRequestHasBeenProccessed, result)
                    .Debug();
                var msg = await result as string;
                await context.PostAsync(msg);
                context.Wait(WelcomeCommuter);
            }
            catch (Exception ex)
            {
                new BotLogger<object>(OperationId, WelcomeFlowId, EventCodes.ExceptionAfterProcessingUserRequest, result, ex)
                    .Exception();
                context.Fail(ex);
            }
        }
    }
}