using Newtonsoft.Json;

namespace Bot.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJsonString(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}