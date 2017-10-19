using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading;
using Bot.Models.Facebook;
using Newtonsoft.Json;
using Bot.Data;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            await context.PostAsync("Hi! Welcome to CorporatePool. Let's set your account with us");

            await context.Forward(new OnboardingDialog(), this.AfterOnboarding, activity, CancellationToken.None);
        }

        private async Task AfterOnboarding(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Welcome aboard mate!!");
            context.Wait(GetUserRequest);
        }

        private async Task AfterTrip(IDialogContext context, IAwaitable<object> result)
        {
            var dialogResult = await result as string;

            if (dialogResult == "owner not onboarded")
            {
                await context.PostAsync("We can fix that");
                await context.Forward(new OnboardingDialog(), this.AfterOnboarding, result, CancellationToken.None);
            }
            else
            {
                await context.PostAsync("Trip completed!!");
                context.Wait(GetUserRequest);
            }
        }

        public async Task GetUserRequest(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;
            
            if (message.Text.ToLower().Contains("going") || message.Text.ToLower().Contains("leaving"))
            {
                await context.Forward(new TripDialog(), this.AfterTrip, message, CancellationToken.None);
            }
            context.Done("");
        }

        public async Task GetUserInformation(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            FacebookMessage message = JsonConvert.DeserializeObject<FacebookMessage>(activity.ChannelData.ToString());

            await context.PostAsync($"User : " + JsonConvert.SerializeObject(Users.UsersList.GetUser(message.sender.id)));

            context.Wait(MessageReceivedAsync);
        }
    }
}