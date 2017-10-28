using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    public class GetPoolersInTrip : MessageHandler<GetPoolersInTripMessage>
    {
        EngineCoreSingleRequest _core = new EngineCoreSingleRequest();

        public override void Handle(GetPoolersInTripMessage message)
        {
            try
            {
                message.Callback(_core.GetPoolersInTrip(message.TripRequest));
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                throw;
            }

        }
    }
}
