using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Bot.Models.Facebook;
using Newtonsoft.Json;
using Bot.Data;
using Bot.Data.Models;
using Bot.Logger;
using Bot.Common;
using Bot.Data.DataManagers;
using Bot.NewData.Enums;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class OnboardingDialog : IDialog<object>
    {
        private string Office = "";
        private string HomeLocation = "";

        Guid OperationId { get; set; }
        Guid OnboardingFlowId { get; set; }

        public OnboardingDialog(Guid operationId, Guid flowId)
        {
            OperationId = operationId;
            OnboardingFlowId = flowId;
        }
        public OnboardingDialog()
        {
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(StartOnboarding);
            return Task.CompletedTask;
        }

        private async Task CheckNextOnboardingStep(IDialogContext context, IAwaitable<NextOnboardingStep> input)
        {
            var nextOnboardingStep = await input;

            switch (nextOnboardingStep)
            {
                case NextOnboardingStep.HomeLocation:
                    await context.PostAsync("Where do you live?"); // this might not be needed if aad is giving this information
                    context.Wait(WhereDoYouLive);
                    break;
                case NextOnboardingStep.OfficeLocation:
                    await context.PostAsync("Where is your office?"); // this might not be needed if aad is giving this information
                    context.Wait(SelectYourOffice);
                    break;

                case NextOnboardingStep.VehicleInformation:
                    await context.PostAsync("Do you own a vehicle?"); 
                    context.Wait(DoYouOwnVehicle);
                    break;
            }
        }


        private async Task StartOnboarding(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            try
            {
                new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.StartingOnboardingProcess, activity)
                    .Debug();

                var commuter = new Commuter(OperationId);
                commuter.CommuterName = activity.From.Name;
                commuter.MediaId = activity.From.Id;
                commuter.ChannelId = activity.ChannelId;
                commuter.CommuterId = Guid.NewGuid();
                CommuterManager.Instance.AddCommuter(OnboardingFlowId, commuter);

                var botChannelConfig = new BotChannelConfig(OperationId)
                {
                    ChannelId = activity.ChannelId,
                    BotId = activity.Recipient.Id,
                    BotName = activity.Recipient.Name,
                    ServiceUrl = activity.ServiceUrl
                };

                BotChannelConfigManager.Instance.AddBotChannelConfig(botChannelConfig);

                // should check if the vehicle has been onboarded
                await context.PostAsync("Do you own a vehicle?(yes/no)");
                context.Wait(DoYouOwnVehicle);
            }
            catch (Exception ex)
            {
                new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.ExceptionWhileStartingOnboarding, activity, ex)
                    .Exception();
                context.Fail(ex);
            }
        }

        private async Task DoYouOwnVehicle(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            try
            {
                new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.AskingForVehicleInformation, activity)
                    .Debug();

                var msgText = activity.Text == null ? string.Empty : activity.Text;
                var commuter = CommuterManager.Instance.GetCommuter(activity.From.Id).ResultData;

                // can we get the previous method
                if (commuter.Vehicle == null || !commuter.Vehicle.VehicleOnboarded)
                {
                    switch (msgText.ToLower())
                    {
                        case "yes":
                            commuter.Vehicle = new Vehicle();
                            new BotLogger<Commuter>(OperationId, OnboardingFlowId, EventCodes.UserWantsToAddVehicleInformation, commuter)
                                .Debug();
                            await context.PostAsync("How many passengers can travel in your vehicle?");
                            context.Wait(HowManyCommuters);
                            break;
                        case "no":
                            commuter.Vehicle = new Vehicle();
                            new BotLogger<Commuter>(OperationId, OnboardingFlowId, EventCodes.UserDoesNotWantsToAddVehicleInformation, commuter)
                                .Debug();
                            await context.PostAsync("Choose your office: \n1. Microsoft");
                            context.Wait(SelectYourOffice);
                            break;
                        default:
                            new BotLogger<Commuter>(OperationId, OnboardingFlowId, EventCodes.WrongUserMessage, commuter)
                                .Debug();
                            await context.PostAsync("Do you own a vehicle?(Yes/No)");
                            context.Wait(DoYouOwnVehicle);
                            break;
                    }
                }
                else
                {
                    new BotLogger<Commuter>(OperationId, OnboardingFlowId, EventCodes.VehicleInformationAlreadyOnBoarded, commuter)
                    .Debug();
                    await context.PostAsync("Choose your office: \n1. Microsoft");
                    context.Wait(SelectYourOffice);
                }
            }
            catch (Exception ex)
            {
                new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.ExceptionWhileAddingVehicleInformation, activity, ex)
                    .Exception();
                context.Fail(ex);
            }
        }

        private async Task HowManyCommuters(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            try
            {
                new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.AskingHowManyCommutersCanSitInVehicle, activity)
                    .Debug();
                var msgText = activity.Text == null ? "-1" : activity.Text;
                int count;
                var commuter = CommuterManager.Instance.GetCommuter(activity.From.Id).ResultData;
                if (commuter == null)
                {
                    new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.UserDoesNotExists, activity)
                    {
                        Message = "From HowManyCommuters"
                    }.Error();
                    await context.PostAsync("Starting onboarding process");
                    await StartOnboarding(context, result);
                    return;
                }
                if (commuter.Vehicle == null)
                {
                    commuter.Vehicle = new Vehicle();
                }

                if (Int32.TryParse(msgText, out count))
                {
                    new BotLogger<Commuter>(OperationId, OnboardingFlowId, EventCodes.SettingNumberOfCommutersThatCanSitInVehicle, commuter)
                    .Debug();
                    commuter.Vehicle.MaxPassengerCount = count;
                    await context.PostAsync("Choose your office: \n1. Microsoft");
                    context.Wait(SelectYourOffice);
                    return;
                }

                new BotLogger<string>(OperationId, OnboardingFlowId, EventCodes.UserResponseInvalid, msgText)
                {
                    Message = "Passenger Count " + msgText
                }.Error();
                await context.PostAsync("How many passengers can travel in your vehicle?");
                context.Wait(HowManyCommuters);
            }
            catch (Exception ex)
            {
                new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.ExceptionSettingNumberOfCommutersThatCanSitInVehicle, activity)
                .Exception();
                context.Fail(ex);
            }
        }

        private async Task SelectYourOffice(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            try
            {
                if (!string.IsNullOrWhiteSpace(Office))
                {
                    new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.OfficeAlreadySetAddHome, activity)
                    .Debug();
                    await context.PostAsync("Send your home location");
                    context.Wait(WhereDoYouLive);
                    return;
                }

                var msgText = activity.Text;
                int option;
                var commuter = CommuterManager.Instance.GetCommuter(activity.From.Id).ResultData;
                if (commuter == null)
                {
                    new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.UserDoesNotExists, activity)
                    {
                        Message = "From SelectYourOffice"
                    }.Error();
                    await context.PostAsync("Starting onboarding process");
                    await StartOnboarding(context, result);
                    return;
                }
                if (Int32.TryParse(msgText, out option))
                {
                    switch (option)
                    {
                        case 1:
                            Office = "Microsoft";
                            new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.AddingOfficeOfUser, activity)
                            {
                                Message = "Office : " + Office
                            }.Debug();

                            CommuterManager.Instance.AddOfficeOfCommuter(OnboardingFlowId, commuter, new Coordinate(17.4318848, 78.34318));

                            new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.OfficeAddedAskForHomeLocation, activity)
                            {
                                Message = "Office : " + Office
                            }.Debug();
                            await context.PostAsync("Send your home location");
                            context.Wait(WhereDoYouLive);
                            break;
                        default:
                            new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.UserResponseInvalid, activity)
                            {
                                Message = "user opted for : " + option
                            }.Debug();
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
                        new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.AddingOfficeOfUser, activity)
                        {
                            Message = "Office : " + Office
                        }.Debug();

                        CommuterManager.Instance.AddOfficeOfCommuter(OnboardingFlowId, commuter, new Coordinate(17.4318848, 78.34318));

                        new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.OfficeAddedAskForHomeLocation, activity)
                        {
                            Message = "Office : " + Office
                        }.Debug();

                        await context.PostAsync("Send your home location");
                        context.Wait(WhereDoYouLive);
                        return;
                    }
                    new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.UserResponseInvalid, activity)
                    {
                        Message = "user opted for : " + msgText
                    }.Debug();
                    await context.PostAsync("Choose your office: \n1. Microsoft");
                    context.Wait(SelectYourOffice);
                }
            }
            catch (Exception ex)
            {
                new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.ExceptionWhileSettingOfficeAddress, activity, ex)
                .Exception();
                context.Fail(ex);
            }
        }

        private async Task WhereDoYouLive(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            try
            {
                if (!string.IsNullOrWhiteSpace(HomeLocation))
                {
                    new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.HomeLocationAlreadyOnboarded, activity)
                    .Debug();

                    context.Done("Onboarding is already done");
                    return;
                }
                FacebookMessage facebookMessage = JsonConvert.DeserializeObject<FacebookMessage>(activity.ChannelData.ToString());

                if (facebookMessage == null)
                {
                    new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.UnableToGeFacebookMessage, activity)
                    .Error();
                    await context.PostAsync("Send your home location");
                    context.Wait(WhereDoYouLive);
                }
                else
                {
                    new BotLogger<FacebookMessage>(OperationId, OnboardingFlowId, EventCodes.GettingCommuterFromFacebookId, facebookMessage)
                    .Debug();

                    var commuter = CommuterManager.Instance.GetCommuter(activity.From.Id).ResultData;
                    var homeCoordinate = new Coordinate(facebookMessage.message.attachments[0].payload.coordinates.lat,
                        facebookMessage.message.attachments[0].payload.coordinates.@long);

                    new BotLogger<Commuter>(OperationId, OnboardingFlowId, EventCodes.SettingHomeLocationOfCommuter, commuter)
                    .Debug();
                    CommuterManager.Instance.AddHouseOfCommuter(OnboardingFlowId, commuter, homeCoordinate);
                }
                new BotLogger<FacebookMessage>(OperationId, OnboardingFlowId, EventCodes.OnboardingIsComplete, facebookMessage)
                    .Debug();
                context.Done("Onboarding is complete");
            }
            catch (Exception ex)
            {
                new BotLogger<Activity>(OperationId, OnboardingFlowId, EventCodes.ExceptionWhileSettingHomeAddress, activity, ex)
                    .Exception();
                context.Fail(ex);
            }
        }
    }
}