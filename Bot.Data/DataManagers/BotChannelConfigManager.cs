using System;
using System.Collections.Concurrent;
using Bot.Data.Models;

namespace Bot.Data.DataManagers
{
    [Serializable]
    public class BotChannelConfigManager
    {
        private static BotChannelConfigManager _botChannelConfigManager;
        public static BotChannelConfigManager Instance => _botChannelConfigManager ?? new BotChannelConfigManager();

        private BotChannelConfigManager()
        {
            _botChannelConfigs = new ConcurrentDictionary<string, BotChannelConfig>();
        }

        private ConcurrentDictionary<string, BotChannelConfig> _botChannelConfigs;

        public void AddBotChannelConfig(BotChannelConfig botChannelConfig)
        {
            _botChannelConfigs.TryAdd(botChannelConfig.ChannelId, botChannelConfig);
        }

        public BotChannelConfig GetBotConfigForChannel(string channelId)
        {
            return _botChannelConfigs[channelId];
        }
    }
}
