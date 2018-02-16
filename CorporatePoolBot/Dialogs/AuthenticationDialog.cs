using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class AuthenticationDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(CheckLogin);
        }

        public async Task CheckLogin(IDialogContext context, IAwaitable<object> result)
        {
            
        }
    }
}