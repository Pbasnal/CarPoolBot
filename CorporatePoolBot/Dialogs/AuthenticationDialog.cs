using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using System.Threading;
using AuthBot.Dialogs;
using Microsoft.Bot.Connector;
using AuthBot;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class AuthenticationDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(CheckLogin);
            return Task.CompletedTask;
        }

        public async Task CheckLogin(IDialogContext context, IAwaitable<object> result)
        {
            var token = await context.GetAccessToken("https://graph.microsoft.com");

            var activity = await result;
            if (string.IsNullOrWhiteSpace(token))
                await context.Forward(new AzureAuthDialog("https://graph.microsoft.com"), LoginResult, activity, CancellationToken.None);
            else
                await context.PostAsync(token);
        }

        public async Task LoginResult(IDialogContext context, IAwaitable<string> result)
        {
            //on success, store login details and don't ask them again but keep the details verified
            var activity = await result;
            await context.PostAsync(activity);
            context.Done("this is done");
        }
    }
}