using System;
using System.ComponentModel.DataAnnotations;
using Bot.Data.Models;

namespace Bot.Data.EfModels
{
    public class EfBotChannelConfig
    {
        [Key]
        public string ChannelId { get; set; }
        public string BotId { get; set; }
        public string BotName { get; set; }
        public string ServiceUrl { get; set; }

        public EfBotChannelConfig()
        {
        }

        public EfBotChannelConfig(BotChannelConfig botChannelConfig)
        {
            ChannelId = botChannelConfig.ChannelId;
            BotId = botChannelConfig.BotId;
            BotName = botChannelConfig.BotName;
            ServiceUrl = botChannelConfig.ServiceUrl;
        }

        public static explicit operator EfBotChannelConfig(BotChannelConfig botChannelConfig)
        {
            return new EfBotChannelConfig(botChannelConfig);
        }

        public BotChannelConfig GetBotChannelConfig(Guid operationId, Guid flowId)
        {
            return new BotChannelConfig(operationId)
            {
                ChannelId = ChannelId,
                BotId = BotId,
                BotName = BotName,
                ServiceUrl = ServiceUrl
            };
        }
    }
}
