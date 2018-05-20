using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bot.NewData.Enums;

namespace Bot.NewData.Models
{
    public class Commuter
    {
        public Guid CommuterId { get; set; }
        public string MediaId { get; set; }
        public string ChannelId { get; set; }
        public AadUserInfo AadInfo { get; set; }
        public NextOnboardingStep NextOnboardingStep { get; set; }
    }
}
