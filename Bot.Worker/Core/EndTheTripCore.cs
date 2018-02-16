using Bot.Common;
using Bot.Data;
using Bot.Data.Models;
using Bot.Logger;
using Bot.Models.Internal;
using Bot.Worker.Models;
using System;
using Bot.Worker.Messages;

namespace Bot.Worker.Core
{
    public class EndTheTripCore
    {
        private EngineState _engineState;

        public EndTheTripCore()
        {
            _engineState = EngineState.Instance;
        }

        public EngineState EngineState
        {
            get
            {
                return _engineState?.Clone();
            }
        }

        public MethodResponse CompleteCommuterRequest(EndTripMessage message)
        {
            var request = message.TripRequest;
            new BotLogger<TripRequest>(request.OperationId, message.MessageId, EventCodes.CompletingCommuterRequest, request)
                .Debug();

            var commuterId = request.Commuter.CommuterId;
            if (request.GoingHow == GoingHow.Drive)
            {
                new BotLogger<TripRequest>(request.OperationId, message.MessageId, EventCodes.RemovingOwnerRequestFromState, request)
                    .Debug();
                //todo: if the result is false, store request for later processing and inform customer
                return _engineState.RemoveTripFromState(request.Commuter);
            }

            Guid tripOwnerId;
            if (!_engineState.PoolerCommuterMapping.TryGetValue(commuterId, out tripOwnerId))
            {
                return new MethodResponse { IsSuccess = false };
            }

            new BotLogger<TripRequest>(request.OperationId, message.MessageId, EventCodes.RemovingPoolerRequestsFromState, request)
                   .Debug();
            return _engineState.RemovePoolerRequestFromState(tripOwnerId, commuterId);
        }
    }
}
