using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Bot.Models.Facebook;
using Newtonsoft.Json;
using Bot.Data;
using Bot.Data.Models;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class OnboardingDialog : IDialog<object>
    {
        private string Office = "";
        private string HomeLocation = "";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(StartOnboarding);
            return Task.CompletedTask;
        }

        private async Task StartOnboarding(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                var commuter = new Commuter();
                commuter.CommuterName = activity.From.Name;
                commuter.MediaId = activity.From.Id;
                commuter.CommuterId = Guid.NewGuid();

                CommuterManager.AddCommuter(commuter);

                // should check if the vehicle has been onboarded
                await context.PostAsync("Do you own a vehicle?(yes/no)");
                //context.Wait(DoYouOwnVehicle);
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }

        //private async Task WhatIsYourName(IDialogContext context, IAwaitable<object> result)
        //{
        //    try
        //    {
        //        var activity = await result as Activity;
        //        var msgText = activity.Text;

        //        if (string.IsNullOrWhiteSpace(CommuterName))
        //        {
        //            if (string.IsNullOrWhiteSpace(msgText))
        //            {
        //                await context.PostAsync("What is your Name?");
        //                context.Wait(WhatIsYourName);
        //                return;
        //            }
        //            var commuter = new Commuter();
        //            commuter.CommuterName = msgText;
        //            await context.PostAsync("Do you own a vehicle?(yes/no)");
        //            context.Wait(DoYouOwnVehicle);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var s = ex.Message;
        //        context.Fail(ex);
        //    }
        //}

        private async Task DoYouOwnVehicle(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                var msgText = activity.Text == null ? string.Empty : activity.Text;
                var commuter = CommuterManager.GetCommuter(activity.From.Id);
                // can we get the previous method

                if (commuter.Vehicle == null || !commuter.Vehicle.VehicleOnboarded)
                {   
                    switch (msgText.ToLower())
                    {
                        case "yes":
                            commuter.Vehicle = new Vehicle();
                            await context.PostAsync("How many passengers can travel in your vehicle?");
                            context.Wait(HowManyCommuters);
                            break;
                        case "no":
                            commuter.Vehicle = new Vehicle();
                            await context.PostAsync("Choose your office: \n1. Microsoft");
                            context.Wait(SelectYourOffice);
                            break;
                        default:
                            await context.PostAsync("Do you own a vehicle?(Yes/No)");
                            context.Wait(DoYouOwnVehicle);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }

        private async Task HowManyCommuters(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                var msgText = activity.Text == null ? "-1" : activity.Text;
                int count;
                var commuter = CommuterManager.GetCommuter(activity.From.Id);
                if (commuter.Vehicle != null || !commuter.Vehicle.VehicleOnboarded)
                {
                    if (Int32.TryParse(msgText, out count))
                    {
                        commuter.Vehicle.MaxPassengerCount = count;
                        await context.PostAsync("Choose your office: \n1. Microsoft");
                        context.Wait(SelectYourOffice);
                        return;
                    }
                    await context.PostAsync("How many passengers can travel in your vehicle?");
                    context.Wait(HowManyCommuters);
                }
                context.Done("Ho gaya");
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }

        private async Task SelectYourOffice(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Office))
                {
                    await context.PostAsync("Send your home location");
                    context.Wait(WhereDoYouLive);
                    return;
                }

                var activity = await result as Activity;
                var msgText = activity.Text;
                int option;
                var commuter = CommuterManager.GetCommuter(activity.From.Id);

                if (Int32.TryParse(msgText, out option))
                {
                    switch (option)
                    {
                        case 1:
                            Office = "Microsoft";
                            CommuterManager.AddOfficeOfCommuter(commuter, new Coordinate(17.4318848, 78.34318));
                            await context.PostAsync("Send your home location");
                            context.Wait(WhereDoYouLive);
                            break;
                        default:
                            await context.PostAsync("Choose your office: \n1. Microsoft");
                            context.Wait(SelectYourOffice);
                            break;
                    }
                }
                else 
                {
                    if (msgText.ToLower().Equals("microsoft"))
                    {
                        Office = "Microsoft";
                        CommuterManager.AddOfficeOfCommuter(commuter, new Coordinate(17.4318848, 78.34318));
                        await context.PostAsync("Send your home location");
                        context.Wait(WhereDoYouLive);
                        return;
                    }
                    await context.PostAsync("Choose your office: \n1. Microsoft");
                    context.Wait(SelectYourOffice);
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }

        private async Task WhereDoYouLive(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(HomeLocation))
                {
                    context.Done("Onboarding is already done");
                    return;
                }
                var activity = await result as Activity;
                FacebookMessage facebookMessage = JsonConvert.DeserializeObject<FacebookMessage>(activity.ChannelData.ToString());

                //Users.UsersList.GetUser(facebookMessage.sender.id);
                if (facebookMessage == null)
                {
                    await context.PostAsync("Send your home location");
                    context.Wait(WhereDoYouLive);
                }
                else
                {
                    var commuter = CommuterManager.GetCommuter(activity.From.Id);
                    var homeCoordinate = new Coordinate(facebookMessage.message.attachments[0].payload.coordinates.lat,
                        facebookMessage.message.attachments[0].payload.coordinates.@long);

                    CommuterManager.AddHouseOfCommuter(commuter, homeCoordinate);
                }
                context.Done("Onboarding is complete");
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                context.Fail(ex);
            }
        }
    }
}