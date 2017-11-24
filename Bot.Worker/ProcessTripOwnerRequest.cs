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

        public ProcessTripOwnerRequest()
        {
            _core = new EngineCoreSingleRequest();
        }

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
