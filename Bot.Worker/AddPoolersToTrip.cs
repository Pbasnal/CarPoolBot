using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    public class AddPoolersToTrip : MessageHandler<AddPoolersToTripMessage>
    {
        private EngineCoreSingleRequest _core;

        public AddPoolersToTrip()
        {
            _core = new EngineCoreSingleRequest();
        }

        public override void Handle(AddPoolersToTripMessage message)
        {
            try
            {
                _core.AddPoolersToTrip(message);
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                throw;
            }
        }
        
    }
}
