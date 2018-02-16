using System;
using Bot.Common;
using Bot.Data.DataManagers;
using Bot.Logger;
using Bot.MessagingFramework;
using Bot.Worker.Messages;
using Microsoft.Bot.Connector;

namespace CorporatePoolBot.MessageHandlers
{
    public class VehicleOwnerAddedToStateHandler : MessageHandler<VehicleOwnerAddedToStateMessage>
    {
        public override void Handle(VehicleOwnerAddedToStateMessage message)
        {
            new BotLogger<VehicleOwnerAddedToStateMessage>(message.OperationId, message.MessageId,
                EventCodes.VehicleOwnerAddedtoStateBegin, message).Debug();

            var commuter = CommuterManager.Instance.GetCommuter(message.TripRequest.Commuter.CommuterId);

            if (commuter == null)
            {
                new BotLogger<VehicleOwnerAddedToStateMessage>(message.OperationId, message.MessageId,
                    EventCodes.VehicleOwnerIsNotInState, message).Debug();
                throw new Exception(EventCodes.VehicleOwnerIsNotInState);
            }

            var botChannel = BotChannelConfigManager.Instance.GetBotConfigForChannel(commuter.ChannelId);
            if (botChannel == null)
            {
                new BotLogger<VehicleOwnerAddedToStateMessage>(message.OperationId, message.MessageId,
                    EventCodes.BotChannelConfigNotFound, message).Debug();
                throw new Exception(EventCodes.BotChannelConfigNotFound);
            }

            var userAccount = new ChannelAccount(commuter.MediaId, commuter.CommuterName);
            var botAccount = new ChannelAccount(botChannel.BotId, botChannel.BotName);

            var connector = new ConnectorClient(new Uri(botChannel.ServiceUrl));
        }
    }
}
