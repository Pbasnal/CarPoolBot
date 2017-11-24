namespace Bot.Models.Internal
{
    public class MethodResponse
    {
        public bool IsSuccess { get; set; }

        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

        public MethodResponse()
        { }

        public MethodResponse(bool isSuccess, int responseCode)
        {
            IsSuccess = isSuccess;
            ResponseCode = responseCode;
        }

        public MethodResponse(bool isSuccess, int responseCode, string responseMessage)
        {
            IsSuccess = isSuccess;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
        }

    }
}
