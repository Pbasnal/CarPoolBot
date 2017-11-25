using System;
using System.Diagnostics;

namespace Bot.Models.Internal
{
    public struct MethodResponse<T>
    {
        //StackTrace stackTrace = new StackTrace();
        StackTrace stackTrace;

        public bool IsSuccess { get; set; }

        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        private T _resultData;
        public T ResultData
        {
            get
            {
                if (_resultData == null)
                {
                    throw new ArgumentNullException();
                }
                return _resultData;
            }
            set
            {
                if (value == null)
                {
                    ResultDataType = string.Empty;
                    return;
                }
                ResultDataType = value.GetType().Name;
                _resultData = value;
            }
        }


        public string ResultDataType { get; private set; }

        //public MethodResponse()
        //{ }

        public MethodResponse(T data)
        {
            stackTrace = new StackTrace();
            _resultData = default(T);
            ResultDataType = string.Empty;
            ResponseMessage = string.Empty;

            if (data == null)
            {
                IsSuccess = false;
                ResponseCode = ResponseCodes.InvalidResponseObject;
                ResponseMessage = ResponseMessages.InvalidResponseObject + " "
                    + data.GetType().Name + " in "
                    + stackTrace.GetFrame(1).GetMethod().Name;
                ResultData = default(T);
            }
            else
            {
                IsSuccess = true;
                ResponseCode = ResponseCodes.SuccessDoNotRetry;
                ResultData = data;
            }
        }

        public MethodResponse(bool isSuccess, int responseCode, T data)
        {
            stackTrace = new StackTrace();
            _resultData = default(T);
            ResultDataType = string.Empty;
            ResponseMessage = string.Empty;

            IsSuccess = isSuccess;
            ResponseCode = responseCode;
            ResultData = data;
        }

        public MethodResponse(bool isSuccess, int responseCode)
        {
            stackTrace = new StackTrace();
            _resultData = default(T);
            ResultDataType = string.Empty;
            ResponseMessage = string.Empty;
            
            IsSuccess = isSuccess;
            ResponseCode = responseCode;
        }

        public MethodResponse(bool isSuccess, int responseCode, string responseMessage)
        {
            stackTrace = new StackTrace();
            _resultData = default(T);
            ResultDataType = string.Empty;
            ResponseMessage = string.Empty;

            IsSuccess = isSuccess;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
        }

    }

    public struct MethodResponse
    {
        public bool IsSuccess { get; set; }

        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public MethodResponse(bool isSuccess, int responseCode)
        {
            ResponseMessage = string.Empty;
            IsSuccess = isSuccess;
            ResponseCode = responseCode;
        }

        public MethodResponse(bool isSuccess, int responseCode, string responseMessage)
        {
            ResponseMessage = string.Empty;
            IsSuccess = isSuccess;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
        }

    }
}
