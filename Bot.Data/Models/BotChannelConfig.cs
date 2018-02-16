using System;

namespace Bot.Data.Models
{
    public class BotChannelConfig : ModelBase
    {
        public string ChannelId { get; set; }
        public string BotId { get; set; }
        public string BotName { get; set; }
        public string ServiceUrl { get; set; }
        public BotChannelConfig(Guid operationId) : base(operationId)
        {
        }
    }
}
