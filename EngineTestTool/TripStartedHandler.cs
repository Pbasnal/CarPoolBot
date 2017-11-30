﻿using System;
using Bot.MessagingFramework;
using Bot.Worker.Messages;

namespace EngineTestTool
{
    public class TripStartedHandler : MessageHandler<TripStartedMessage>
    {
        static int TotalProcessed = 0;

        public TripStartedHandler(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
        }

        public override void Handle(TripStartedMessage message)
        {
            Program.Map.AddCommuterAndPassengersToList(message);
            Program.Map.UpdateState(TotalProcessed++);
            //Program.Map.DisplayTrip(message);
        }
    }
}
