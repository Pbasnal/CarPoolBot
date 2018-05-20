using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using System.Threading;
using AuthBot.Dialogs;
using Microsoft.Bot.Connector;
using AuthBot;
using AuthBot.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Bot.NewData.DataManagers;
using Bot.NewData.Models;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class AuthenticationDialog : IDialog<object>
    {

        private string _askForLoginMessage = "You need to login to proceed";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(IsAnExistingUser);
        }

        public async Task IsAnExistingUser(IDialogContext context, IAwaitable<object> activityAsync)
        {
            var activity = await activityAsync as Activity;

            var tkn = await context.GetAccessToken("https://graph.microsoft.com");

            if (string.IsNullOrWhiteSpace(tkn))
            {
                context.UserData.SetValue("ChannelId", activity.ChannelId);
                context.UserData.SetValue("MediaId", activity.From.Id);
                await AuthenticateNewUser(context, activity);
                return;
            }

            //Validate user ad account
            var aadUser = await GetUserInformation(context.UserData.GetValue<AuthResult>("authResult"));
            if (aadUser == null)
            {
                await context.PostAsync("Your details were not found. Please login again");
                await context.Logout();
                Commuters.RemoveUser(activity.ChannelId, activity.From.Id);
                await AuthenticateNewUser(context, activity);
                return;
            }

            var commuter = Commuters.GetCommuterViaChannelIdAndMediaId(activity.ChannelId, activity.From.Id);
            if (commuter == null)
            {
                Commuters.AddOrUpdateAadUser(activity.ChannelId, activity.From.Id, aadUser);
            }

            await ForwardToWelcomeDialog(context);
        }

        private async Task ForwardToWelcomeDialog(IDialogContext context)
        {
            await context.PostAsync("Welcome");
            await context.Forward(new WelcomeDialog(Guid.NewGuid(), Guid.NewGuid()), EndConversation, string.Empty, CancellationToken.None);
            context.Done("Done");
        }

        private async Task EndConversation(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("See you later");
            context.Done("See you later");
        }

        public async Task AuthenticateNewUser(IDialogContext context, Activity activity)
        {
            await context.PostAsync(_askForLoginMessage);
            await context.Forward(new AzureAuthDialog("https://graph.microsoft.com"), LoginResult, activity, CancellationToken.None);
        }

        public async Task LoginResult(IDialogContext context, IAwaitable<string> result)
        {
            if (GetUserinfoAndAddUser(context) == null)
            {
                context.Fail(new Exception("Wrong user details. Please try again"));
                return;
            }

            await ForwardToWelcomeDialog(context);
        }
        private AadUserInfo GetUserinfoAndAddUser(IDialogContext context)
        {
            var aadUser = GetUserInformation(context.UserData.GetValue<AuthResult>("authResult")).Result;

            if (aadUser == null)
            {
                context.Fail(new Exception("Authentication failed to get UserInfo. Please provide correct details"));
                return null;
            }

            Commuters.AddOrUpdateAadUser(context.UserData.GetValue<string>("ChannelId"),
                context.UserData.GetValue<string>("MediaId"), aadUser);
            return aadUser;
        }

        static async Task<AadUserInfo> GetUserInformation(AuthResult authResult)
        {
            var client = new HttpClient();

            var uri = "https://graph.microsoft.com/v1.0/d594eaba-a12b-4cb1-ba6e-b6d65b4879fb/users/" + authResult.UserUniqueId;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            var response = client.GetAsync(uri).Result;

            if (response.Content != null)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AadUserInfo>(responseString.ToString());
            }
            return null;
        }
    }
}