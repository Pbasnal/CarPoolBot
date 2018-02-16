using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using System.Threading;
using AuthBot.Dialogs;
using Microsoft.Bot.Connector;

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
            await context.Forward(new AzureAuthDialog("https://graph.microsoft.com"), LoginResult, context, CancellationToken.None);
        }

        public async Task LoginResult(IDialogContext context, IAwaitable<object> result)
        {
            //on success, store login details and don't ask them again but keep the details verified
            var activity = await result as Activity;
            context.Done("this is done");
        }
    }
}