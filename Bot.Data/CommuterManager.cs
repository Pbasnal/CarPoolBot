using System;
using System.Collections.Generic;

namespace Bot.Data
{
    [Serializable]
    public class CommuterManager
    {
        private static IDictionary<string, Commuter> commuters = null;

        private CommuterManager()
        { }

        public static IDictionary<string, Commuter> CommutersList {
            get
            {
                if (commuters == null)
                    commuters = new Dictionary<string, Commuter>();
                return commuters;
            }
            set
            {}
        }

        public static void AddMicrosotCommuter(Commuter commuter)
        {
            var office = new Coordinate();
            office.Latitude = 17.4318848;
            office.Longitude = 78.34318;

            commuter.OfficeCoordinate = office;

            commuters.Add(commuter.CommuterId, commuter);
        }

        public static Commuter GetCommuter(string commuterId)
        {
            if (commuters == null || commuters.Count == 0)
                return null;
            return commuters[commuterId];
        }
    }
}
