using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Bot.Data;
using Bot.Data.Models;
using Bot.MessagingFramework;
using Bot.Worker.Messages;
using Bot.Logger;
using Bot.Common;
using Bot.Models.Internal;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class RequestDialog : IDialog<object>
    {
        private GoingTo GoingTo = GoingTo.Nowhere;
        private GoingHow GoingHow = GoingHow.Walk;

        private Guid OperationId { get; set; }
        private Guid RequestFlowId { get; set; }

        public RequestDialog(Guid operationId, Guid flowId)
        {
            OperationId = operationId;
            RequestFlowId = flowId;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(WhereDoYouWantToGo);
            return Task.CompletedTask;
        }

        private async Task WhereDoYouWantToGo(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                new BotLogger<object>(OperationId, RequestFlowId, EventCodes.UserResponseForWhereDoYouWantToGo, result)
                    .Debug();

                var activity = await result as Activity;
                var msgText = activity.Text.ToLower();

                if (GoingTo != GoingTo.Nowhere)
                {
                    new BotLogger<Activity>(OperationId, RequestFlowId, EventCodes.UserIsAlreadyGoingSomewhere, activity)
                    {
                        Message = "GoingTo : " + GoingTo.ToString()
                    }.Debug();
                    return;
                }
                else if (msgText.Equals("office"))
                {
                    GoingTo = GoingTo.Office;
                    new BotLogger<string>(OperationId, RequestFlowId, EventCodes.UserWantsToGoToOffice, msgText)
                    {
                        Message = "GoingTo : " + GoingTo.ToString()
                    }.Debug();
                }
                else if (msgText.Equals("home"))
                {
                    GoingTo = GoingTo.Home;
                    new BotLogger<string>(OperationId, RequestFlowId, EventCodes.UserWantsToGoHome, msgText)
                    {
                        Message = "GoingTo : " + GoingTo.ToString()
                    }.Debug();
                }
                else
                {
                    new BotLogger<string>(OperationId, RequestFlowId, EventCodes.UserResponseInvalid, msgText)
                    {
                        Message = "GoingTo : " + msgText
                    }.Error();
                    await context.PostAsync("Where do you want to go? Office or Home?");
                    context.Wait(WhereDoYouWantToGo);
                    return;
                }
                await context.PostAsync("Will start the trip to " + GoingTo.ToString() + ". Do you want to drive or join?");
                context.Wait(WhichTransportMode);
            }
            catch (Exception ex)
            {
                new BotLogger<object>(OperationId, RequestFlowId, EventCodes.UserWantsToGoHome, result, ex)
                    .Exception();
                context.Fail(ex);
            }
        }

        private async Task WhichTransportMode(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                new BotLogger<object>(OperationId, RequestFlowId, EventCodes.AskingForTransportMode, result)
                    .Debug();

                var activity = await result as Activity;
                var msgText = activity.Text.ToLower();

                if (GoingHow != GoingHow.Walk)
                {
                    new BotLogger<Activity>(OperationId, RequestFlowId, EventCodes.UserHasAlreadyDecidedOnGoingHow, activity)
                    {
                        Message = "GoingHow: " + GoingHow
                    }.Debug();
                    return;
                }
                if (msgText.Equals("drive"))
                {
                    GoingHow = GoingHow.Drive;
                    new BotLogger<string>(OperationId, RequestFlowId, EventCodes.UserDecidedToDrive, msgText)
                    {
                        Message = "GoingHow: " + GoingHow
                    }.Debug();
                }
                else if (msgText.Equals("join"))
                {
                    GoingHow = GoingHow.Pool;
                    new BotLogger<string>(OperationId, RequestFlowId, EventCodes.UserDecidedToJoin, msgText)
                    {
                        Message = "GoingHow: " + GoingHow
                    }.Debug();
                }
                else
                {
                    new BotLogger<string>(OperationId, RequestFlowId, EventCodes.UserResponseInvalid, msgText)
                    {
                        Message = "GoingHow: " + GoingHow
                    }.Error();
                    await context.PostAsync("Do you want to drive or join?");
                    context.Wait(WhichTransportMode);
                    return;
                }

                await context.PostAsync("You have chosen to " + GoingHow);
                CreateTripRequest(activity);
                context.Done("User request created");

            }
            catch (Exception ex)
            {
                new BotLogger<object>(OperationId, RequestFlowId, EventCodes.ExceptionWhileDecidingTransportMode, result, ex)
                    .Exception();
                context.Fail(ex);
            }
        }

        private void CreateTripRequest(Activity activity)
        {
            new BotLogger<Activity>(OperationId, RequestFlowId, EventCodes.CreatingTripRequestForUser, activity)
                .Debug();

            TripRequest request = new TripRequest(OperationId, RequestFlowId)
            {
                Commuter = CommuterManager.Instance.GetCommuter(new Guid(activity.From.Id)).ResultData,
                GoingHow = GoingHow,
                GoingTo = GoingTo,
                RequestTime = DateTime.UtcNow,
                WaitTime = TimeSpan.FromMinutes(15)
            };

            

            new BotLogger<TripRequest>(OperationId, RequestFlowId, EventCodes.SendingTripRequestToRequestManager, request)
                .Debug();

            var methodResponse = TripRequestManager.Instance.AddTripRequest(request);
            Tuple<MethodResponse, TripRequest> logObject = new Tuple<MethodResponse, TripRequest>(methodResponse, request);

            if (!methodResponse.IsSuccess)
            {
                var ex = new Exception(methodResponse.ResponseMessage);

                new BotLogger<Tuple<MethodResponse, TripRequest>>(OperationId, RequestFlowId, EventCodes.TripRequestCreationFailed, logObject, ex)
                        .Exception();
                throw ex;
            }

            if (GoingHow == GoingHow.Drive)
            {
                new BotLogger<Tuple<MethodResponse, TripRequest>>(OperationId, RequestFlowId, EventCodes.PublishingOwnerRequestMessage, logObject)
                    .Debug();

                MessageBus.Instance.Publish(new ProcessTripOwnerRequestMessage(OperationId, RequestFlowId)
                {
                    TripOwnerRequest = request
                });
            }

            new BotLogger<Tuple<MethodResponse, TripRequest>>(OperationId, RequestFlowId, EventCodes.TripRequestCreated, logObject)
                .Debug();

            return;
        }
    }
}