namespace Bot.Models.Internal
{
    public class ResponseCodes
    {
        // 1*** for errors, 2*** for success
        public const int InvalidInputParameter = 1001;
        public const int TripStarted = 2001;
        public const int TripDidNotStart = 2001;
        public const int SuccessDoNotRetry = 2002;
        public const int SuccessCanRetry = 2003;
    }
}