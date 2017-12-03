using Bot.MessagingFramework;
using Bot.Models.Internal;
using Bot.Worker.Messages;
using System.Linq;

namespace CorporatePoolBot.MessageHandlers
{
    public class RequestOwnerToAcceptPoolersHandler : MessageHandler<ReqestOwnerToAcceptPoolersMessage>
    {
        public override void Handle(ReqestOwnerToAcceptPoolersMessage message)
        {
            var waitingPoolers = message.PoolersToRequestFor;

            var poolerIndices = Enumerable.Range(0, waitingPoolers.Count);
            MessageBus.Instance.Publish(new AddPoolersToTripMessage(message.OperationId, message.MessageId)
            {
                PoolerIndices = poolerIndices.ToArray(),
                TripRequest = message.OwnerRequest,
                SearchForMorePoolers = (message.Status == ResponseCodes.SuccessCanRetry)
            });
        }
    }
}
