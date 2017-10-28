using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Worker
{
    public class AddPoolersToTrip : MessageHandler<AddPoolersToTripMessage>
    {
        private EngineCoreSingleRequest _core;

        public override void Handle(AddPoolersToTripMessage message)
        {
            try
            {
                _core.AddPoolersToTrip(message.TripRequest, message.PoolerIndices);
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                throw;
            }
        }
        
    }
}
