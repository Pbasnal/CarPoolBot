using Bot.Data;
using Bot.Data.Models;
using Bot.MessagingFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Worker.Messages
{
    public class TripDidNotStartedMessage : MessageBase
    {
        public Trip Trip { get; set; }

        public Models.Route Route { get; set; }
    }
}
