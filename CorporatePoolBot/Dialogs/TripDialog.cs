using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Bot.Data;
using Bot.Models.Facebook;
using Newtonsoft.Json;

namespace CorporatePoolBot.Dialogs
{
    [Serializable]
    public class TripDialog : IDialog<object>
    {
        private GoingTo GoingWhere;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(TripRequestReceived);

            return Task.CompletedTask;
        }
        public async Task TripRequestReceived(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;

            if (message.Text.ToLower().Contains("going"))
            {
                // find a person who can join the user in car while going to office
                GoingWhere = GoingTo.Office;
            }
            else if (message.Text.ToLower().Contains("leaving"))
            {
                // find a person who can join the user in car while going to home
                GoingWhere = GoingTo.Home;
            }

            await context.PostAsync("Are you driving or taking lift");

            context.Wait(CommuterTripMessageReceived);
        }

        public async Task CommuterTripMessageReceived(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;
            Trip trip = null;

            if (message.Text.ToLower().Contains("lift"))
            {
                //find a car for user
                //find a trip which can drop you to destination
            }
            else if (message.Text.ToLower().Contains("drive"))
            {
                //start a new trip for the user
                FacebookMessage fbmessage = JsonConvert.DeserializeObject<FacebookMessage>(message.ChannelData.ToString());
                Commuter owner = CommuterManager.GetCommuter(fbmessage.sender.id);
                if (owner == null)
                {
                    await context.PostAsync("You are not yet onboarded");
                    context.Done("owner not onboarded");
                }
                if (owner.Vehicle == null || owner.Vehicle.MaxPassengerCount - owner.Vehicle.OccupiedSeats == 0)
                {
                    await context.PostAsync("Unable to start the trip. Make sure you have a vehicle. " +
                        JsonConvert.SerializeObject(owner));
                    context.Done("trip did not start");
                    return;
                }
                trip = TripManager.TripsList.StartNewTrip(owner);
                await context.PostAsync("Trip started " + JsonConvert.SerializeObject(trip));
                context.Done("Trip started");
            }
            await context.PostAsync("Did not understood what you meant");
            context.Done("Trip not started");
        }

        public async Task CommuterGoingMessageReceived(IDialogContext context, IAwaitable<object> message)
        {
        }
    }
}