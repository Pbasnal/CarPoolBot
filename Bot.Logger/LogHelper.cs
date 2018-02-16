using Bot.Extensions;

namespace Bot.Logger
{
    public class LogHelper
    {
        public static string CreatePayload(params object[] items)
        {
            string payload = "{";
            foreach (var item in items)
            {
                payload += "\"" + item.GetType().Name + "" +
                           "\": " + item.ToJsonString() + ",";
            }
            payload += "}";

            return payload;
        }
    }
}
