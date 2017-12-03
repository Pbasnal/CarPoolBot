using Bot.Common;
using Bot.Data.Models;
using Bot.Logger;
using Bot.Worker.Models;
using System.Collections.Generic;

namespace Bot.Worker.Core
{
    public class GetPoolersInTripCore
    {
        private EngineState _engineState;

        public GetPoolersInTripCore()
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

        public IList<Commuter> GetPoolersInTrip(TripRequest request)
        {
            var commuterRequestProcess = _engineState.CommuterRequestProcessTable[request.Commuter.CommuterId];

            new BotLogger<CommuterRequestProcessModel>(request.OperationId, request.FlowId, EventCodes.GotProcessModel, commuterRequestProcess)
                .Debug();

            return commuterRequestProcess.Trip.Passengers;
        }
    }
}
