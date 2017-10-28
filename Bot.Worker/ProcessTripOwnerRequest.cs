using Bot.MessagingFramework;
using Bot.Worker.Core;
using Bot.Worker.Messages;
using System;

namespace Bot.Worker
{
    //todo: handle callbacks 
    public class ProcessTripOwnerRequest : MessageHandler<ProcessTripOwnerRequestMessage>
    {
        private EngineCoreSingleRequest _core;

        public override void Handle(ProcessTripOwnerRequestMessage message)
        {
            try
            {
                _core.ProcessRequests(message);
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                throw;
            }
        }
    }
}
