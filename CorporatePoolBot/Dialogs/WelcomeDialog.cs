using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Threading;
using Bot.External;
using Bot.Logger;
using Bot.Common;
using Bot.Models.Internal;
using Bot.NewData.DataManagers;
using Bot.NewData.Enums;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class WelcomeDialog : IDialog<object>
    {
        private string _greeting = "Hi! Welcome to CorPool";
        private string _askForRequest = "How may I help you today?";

        private UserAuthentication userAuthenticator;

        private Guid OperationId { get; set; }
        private Guid WelcomeFlowId { get; set; }

        public WelcomeDialog(Guid operationId, Guid flowId)
        {
            OperationId = operationId;
            WelcomeFlowId = Guid.NewGuid();
        }

        public async Task StartAsync(IDialogContext context)
        {
            userAuthenticator = new UserAuthentication(OperationId, WelcomeFlowId);
            context.Wait(WelcomeCommuter);
        }

        private async Task WelcomeCommuter(IDialogContext context, IAwaitable<object> input)
        {
            try
            {
                var activity = await input as Activity;
                await context.PostAsync(_greeting);
                var commuter = Commuters.GetCommuterViaChannelIdAndMediaId(activity.ChannelId, activity.From.Id);

                if (commuter.NextOnboardingStep != NextOnboardingStep.Complete ||
                    commuter.NextOnboardingStep != NextOnboardingStep.VehicleInformation)
                {
                    await context.Forward(new OnboardingDialog(commuter.NextOnboardingStep), WelcomeCommuter, commuter, CancellationToken.None);
                }

                await context.PostAsync(_askForRequest);
                context.Wait(GetRequestFromUser);
            }
            catch (Exception ex)
            {
                context.Fail(ex);
            }
        }

        public async Task GetRequestFromUser(IDialogContext context, IAwaitable<object> input)
        {
            var activity = await input as Activity;
            await context.Forward(new RequestDialog(OperationId, WelcomeFlowId), AfterProcessingUserRequest, activity, CancellationToken.None);
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