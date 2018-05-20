using Bot.NewData.Models;
using System;
using System.Collections.Generic;

namespace Bot.NewData.DataManagers
{
    public class Commuters
    {
        private static Commuters _instance = new Commuters();
        private List<Commuter> _commuters { get; set; }

        private IDictionary<string, Commuter> _channelmediaAndCommuterMap;

        private Commuters()
        {
            _commuters = new List<Commuter>();
            _channelmediaAndCommuterMap = new Dictionary<string, Commuter>();
        }

        public static Commuter GetCommuterViaChannelIdAndMediaId(string channelId, string mediaId)
        {
            try
            {
                Commuter commuter;
                if (_instance._channelmediaAndCommuterMap == null || !_instance._channelmediaAndCommuterMap.TryGetValue(channelId + mediaId, out commuter))
                    return null;
                return commuter;
            }
            catch (Exception ex)
            {
                //TODO: Log
                throw ex;
            }
        }

        public static Commuter AddOrUpdateAadUser(string channelId, string mediaId, AadUserInfo aadUserInfo)
        {
            try
            {
                var commuter = new Commuter
                {
                    ChannelId = channelId,
                    MediaId = mediaId,
                    CommuterId = Guid.NewGuid(),
                    AadInfo = aadUserInfo,
                    NextOnboardingStep = Enums.NextOnboardingStep.HomeLocation
                };

                // validate if mappings exists
                if (_instance._channelmediaAndCommuterMap.Keys.Contains(channelId + mediaId))
                {
                    var oldCommuter = _instance._channelmediaAndCommuterMap[channelId + mediaId];
                    oldCommuter.AadInfo = commuter.AadInfo;
                    return oldCommuter;
                }

                // add mappings
                _instance._channelmediaAndCommuterMap.Add(channelId + mediaId, commuter);


                // add commuter
                _instance._commuters.Add(commuter);

                return commuter;
            }
            catch (Exception ex)
            {
                //TODO: Log
                throw ex;
            }
        }

        public static void RemoveUser(string channelId, string mediaId)
        {
            try
            {
                var key = channelId + mediaId;
                // validate if mappings exists
                if (_instance._channelmediaAndCommuterMap.Keys.Contains(key))
                    return;

                var commuter = _instance._channelmediaAndCommuterMap[key];
                _instance._channelmediaAndCommuterMap.Remove(key);

                // add commuter
                _instance._commuters.Remove(commuter);
            }
            catch (Exception ex)
            {
                //TODO: Log
                throw ex;
            }
        }
    }
}
