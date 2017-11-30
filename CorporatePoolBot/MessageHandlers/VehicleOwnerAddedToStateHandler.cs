using System;
using Bot.MessagingFramework;
using Bot.Worker.Messages;

namespace CorporatePoolBot.MessageHandlers
{
    public class VehicleOwnerAddedToStateHandler : MessageHandler<VehicleOwnerAddedToStateMessage>
    {
        public VehicleOwnerAddedToStateHandler(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public override void Handle(VehicleOwnerAddedToStateMessage message)
        {
        }
    }
}
