using Bot.Common;
using Bot.Data.Models;
using Bot.Logger;
using Bot.Worker.Messages;
using Bot.Worker.Models;
using System;
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

        public IList<Commuter> GetPoolersInTrip(GetPoolersInTripMessage message)
        {
            var commuterRequestProcess = _engineState.CommuterRequestProcessTable[message.TripRequest.Commuter.CommuterId];

            new BotLogger<CommuterRequestProcessModel>(message.TripRequest.OperationId, message.MessageId, EventCodes.GotProcessModel, commuterRequestProcess)
                .Debug();

            return commuterRequestProcess.Trip.Passengers;
        }
    }
}
