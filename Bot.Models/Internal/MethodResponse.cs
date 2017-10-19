namespace Bot.Models.Internal
{
    public class MethodResponse
    {
        public bool IsSuccess { get; set; }

        public ResponseInfo ResponseInfo { get; set; }

        public MethodResponse()
        { }

        public MethodResponse(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public MethodResponse(bool isSuccess, ResponseCodes responseCode, string responseMessage)
        {
            IsSuccess = isSuccess;
            ResponseInfo = new ResponseInfo
            {
                ResponseCode = responseCode,
                ResponseMessage = responseMessage
            };
        }

    }

    public class ResponseInfo
    {
        public ResponseCodes ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
