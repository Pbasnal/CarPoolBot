using Bot.MessagingFramework;
using Bot.Models.Internal;
using Bot.Worker.Messages;
using System.Linq;
using System;

namespace CorporatePoolBot.MessageHandlers
{
    public class RequestOwnerToAcceptPoolersHandler : MessageHandler<ReqestOwnerToAcceptPoolersMessage>
    {
        public RequestOwnerToAcceptPoolersHandler(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public override void Handle(ReqestOwnerToAcceptPoolersMessage message)
        {
            var waitingPoolers = message.PoolersToRequestFor;

            var poolerIndices = Enumerable.Range(0, waitingPoolers.Count);
            MessageBus.Instance.Publish(new AddPoolersToTripMessage
            {
                PoolerIndices = poolerIndices.ToArray(),
                TripRequest = message.OwnerRequest,
                SearchForMorePoolers = (message.Status == ResponseCodes.SuccessCanRetry)
            });
        }
    }
}
