using Bot.Common;
using Bot.Data.Models;
using Bot.Logger;
using Bot.Worker.Messages;
using Bot.Worker.Models;

namespace Bot.Worker.Core
{
    public class GetTripRouteCore
    {
        private EngineState _engineState;

        public GetTripRouteCore()
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

        public Route GetTripRoute(GetTripRouteMessage message)
        {
            CommuterRequestProcessModel processModel;
            if (_engineState.CommuterRequestProcessTable.TryGetValue(message.TripRequest.Commuter.CommuterId, out processModel))
            {
                new BotLogger<CommuterRequestProcessModel>(message.TripRequest.OperationId, message.MessageId, EventCodes.GotProcessModelForTripRoute, processModel)
                    .Debug();
                return processModel.CompleteRoute;
            }
            return null;
        }
    }
}
