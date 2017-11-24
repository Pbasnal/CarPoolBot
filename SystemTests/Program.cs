using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace SystemTests
{
    static class TokenRequest
    {
        public const string grant_type = "client_credentials";
        public const string client_id = "eb8253d6-0d71-4030-b808-5b58ffef0f87";
        public const string client_secret = "jDqrxbCoY38c5moqEBfNOq1";
        public const string scope = "https://api.botframework.com/.default";
    }

    public class ChatBotApiTests
    {

        static string ApiUrl = @"http://localhost:3979/api/messages";
        string AuthHeader = @"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjJLVmN1enFBaWRPTHFXU2FvbDd3Z0ZSR0NZbyJ9.eyJhdWQiOiJlYjgyNTNkNi0wZDcxLTQwMzAtYjgwOC01YjU4ZmZlZjBmODciLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vZDZkNDk0MjAtZjM5Yi00ZGY3LWExZGMtZDU5YTkzNTg3MWRiL3YyLjAiLCJpYXQiOjE1MTEzNjE3NTIsIm5iZiI6MTUxMTM2MTc1MiwiZXhwIjoxNTExMzY1NjUyLCJhaW8iOiJZMk5nWUZoNnRWTE1lRlp0bFV5NzZJbWZSVDFCQUE9PSIsImF6cCI6ImViODI1M2Q2LTBkNzEtNDAzMC1iODA4LTViNThmZmVmMGY4NyIsImF6cGFjciI6IjEiLCJ0aWQiOiJkNmQ0OTQyMC1mMzliLTRkZjctYTFkYy1kNTlhOTM1ODcxZGIiLCJ1dGkiOiJvVzJyamZSS2ZrQ1BuWVRSQ1dNU0FBIiwidmVyIjoiMi4wIn0.Hcc76qWEUrQ9uGnxZ5WjuL0XXRW4B64dEQMFyJLvWmCqRc-R3Oq8rc8cSEvS7sIQ8H3oM3EnEFHEQwaX2Fle8dtXqLpBLbjv__Vwgj-dpip9x29qHk9zpqntb-KZL2hGnKeF41W0wYRXpSudNjXpUjokkcg09eI1edinEq-n-LFw-_Pk205omApc0hHqlijEnk8oK-jN4ZzRq9lbFQ_tP5VotSiQEcY3Mv0rFJJqsU_aKNwIiPAvdCPan-UPThk8e3S7zMAiJx1aKn2dCyN1TbUWZpyFhmLCRizMUD2l-gT7dLR_miXTT142XtH6EfrApR-17ncJ2ZGqTPdkMsIFuw";
        string Payload = "{\"type\":\"message\",\"text\":\"hi\",\"from\":{\"id\":\"default-user\",\"name\":\"User\"},\"locale\":\"en-US\",\"textFormat\":\"plain\",\"timestamp\":\"2017-11-22T14:51:08.737Z\",\"channelData\":{\"clientActivityId\":\"1511362051642.8240439066339238.6\"},\"id\":\"m9j314jnni9l\",\"channelId\":\"emulator\",\"localTimestamp\":\"2017-11-22T20:21:08+05:30\",\"recipient\":{\"id\":\"4l5hlf96i85fe02b1c\",\"name\":\"Bot\"},\"conversation\":{\"id\":\"1n87e9ek7kf5\"},\"serviceUrl\":\"http://localhost:33199\"}";

        static void Main(string[] args)
        {
            
            var client = new HttpClient();

            HttpClientEx.AddAPIAuthorization(client, TokenRequest.client_id, TokenRequest.client_secret).Wait();

            Activity activity = new Activity
            {
                Text = "hi this is program"
            };

            var result = client.PostAsJsonAsync(ApiUrl, activity).Result;
        }
    }
}
