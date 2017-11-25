using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Bot.Data;
using Bot.Data.Models;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class RequestDialog : IDialog<object>
    {
        private GoingTo GoingTo = GoingTo.Nowhere;
        private GoingHow GoingHow = GoingHow.Walk;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(WhereDoYouWantToGo);
            return Task.CompletedTask;
        }

        private async Task WhereDoYouWantToGo(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                var msgText = activity.Text;

                if (GoingTo == GoingTo.Nowhere)
                {
                    if (msgText.ToLower().Equals("office"))
                    {
                        GoingTo = GoingTo.Office;
                        await context.PostAsync("Will start the trip to Office. Do you want to drive or join?");
                        context.Wait(WhichTransportMode);
                    }
                    else if (msgText.ToLower().Equals("home"))
                    {
                        GoingTo = GoingTo.Home;
                        await context.PostAsync("Will start the trip to home. Do you want to drive or join?");
                        context.Wait(WhichTransportMode);
                    }
                    else
                    {
                        await context.PostAsync("Where do you want to go? Office or Home?");
                        context.Wait(WhereDoYouWantToGo);
                    }
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }

        private async Task WhichTransportMode(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                var msgText = activity.Text;

                if (GoingHow != GoingHow.Walk)
                {
                    if (msgText.ToLower().Equals("drive"))
                    {
                        GoingHow = GoingHow.Drive;
                        await context.PostAsync("You have chosen to drive");
                        CreateTripRequest(activity);
                        context.Done("User request created");
                    }
                    else if (msgText.ToLower().Equals("join"))
                    {
                        GoingHow = GoingHow.Pool;
                        await context.PostAsync("You have chosen to join");
                        CreateTripRequest(activity);
                        context.Done("User request created");
                    }
                    else
                    {
                        await context.PostAsync("Do you want to drive or join?");
                        context.Wait(WhichTransportMode);
                    }
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }

        private void CreateTripRequest(Activity activity)
        {
            TripRequest request = new TripRequest
            {
                Commuter = CommuterManager.Instance.GetCommuter(new Guid(activity.From.Id)).ResultData,
                GoingHow = this.GoingHow,
                GoingTo = this.GoingTo,
                RequestTime = DateTime.UtcNow,
                WaitTime = TimeSpan.FromMinutes(15)
            };

            TripRequestManager.Instance.AddTripRequest(request);
        }
    }
}