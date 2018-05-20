using Bot.Models.Internal;
using System;
using Microsoft.Bot.Connector;
using Bot.Logger;
using Bot.Common;

namespace Bot.External
{
    [Serializable]
    public class UserAuthentication
    {
        Guid OperationId { get; set; }
        Guid AuthenticationFlowId { get; set; }

        public UserAuthentication(Guid operationId, Guid flowId)
        {
            OperationId = operationId;
            AuthenticationFlowId = flowId;
        }

        public MethodResponse Authenticate( Activity activity)
        {
            new BotLogger<Activity>(OperationId, AuthenticationFlowId, EventCodes.AuthenticatingUser, activity).Debug();
            return new MethodResponse
            {
                IsSuccess = false,
                //ResponseCode = ResponseCodes.,
                //ResponseMessage = ResponseMessages.UserAuthenticationSuccessful
            };
        }
    }
}
