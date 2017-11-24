using Bot.Models.Internal;
using System;
using Microsoft.Bot.Connector;
namespace Bot.External
{
    [Serializable]
    public class UserAuthentication
    {
        public MethodResponse Authenticate(Activity activity)
        {
            return new MethodResponse
            {
                IsSuccess = false,
                ResponseCode = ResponseCodes.SuccessDoNotRetry,
                ResponseMessage = ResponseMessages.UserAuthenticationSuccessful
            };
        }
    }
}
