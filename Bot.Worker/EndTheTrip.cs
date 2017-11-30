using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    public class EndTheTrip : MessageHandler<EndTripMessage>
    {
        private EngineCoreSingleRequest _core;

        public EndTheTrip(Guid operationId, Guid flowId) : base(operationId, flowId)
        {
            _core = new EngineCoreSingleRequest();
        }

        public override void Handle(EndTripMessage message)
        {
            try
            {
                _core.CompleteCommuterRequest(message.TripRequest);
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                throw;
            }
        }
    }
}
