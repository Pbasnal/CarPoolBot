using Bot.Data.Models;
using Bot.Models.Internal;
using System;
using System.Collections.Generic;

namespace Bot.Data
{
    //singleton
    [Serializable]
    public class CommuterManager
    {
        private static IDictionary<Guid, Commuter> commuters = new Dictionary<Guid, Commuter>();

        private CommuterManager()
        {}

        public static IDictionary<Guid, Commuter> CommutersList { get; private set; }

        public static void AddMicrosotCommuter(Commuter commuter)
        {
            var office = new Coordinate();
            office.Latitude = 17.4318848;
            office.Longitude = 78.34318;

            commuter.OfficeCoordinate = office;
            commuters.Add(commuter.CommuterId, commuter);

            using (var ctx = new DatabaseContext())
            {
                ctx.Commuters.Add(commuter);
                ctx.SaveChanges();
            }
        }

        public static void AddCommuter(Commuter commuter)
        {
            var office = new Coordinate();
            commuter.OfficeCoordinate = office;
            commuters.Add(commuter.CommuterId, commuter);

            using (var ctx = new DatabaseContext())
            {
                try
                {
                    ctx.Commuters.Add(commuter);
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    var str = ex.Message;
                }

            }
        }

        public static MethodResponse AddOfficeOfCommuter(Commuter inCommuter, Coordinate officeCoordinate)
        {
            Commuter commuter;
            if (!commuters.TryGetValue(inCommuter.CommuterId, out commuter))
                return new MethodResponse(false, ResponseCodes.InvalidInputParameter, ResponseMessages.CommuterDoesNotExists );

            commuter.OfficeCoordinate = officeCoordinate;
            return new MethodResponse(true, ResponseCodes.SuccessDoNotRetry);
        }

        public static MethodResponse AddHouseOfCommuter(Commuter inCommuter, Coordinate homeCoordinate)
        {
            Commuter commuter;
            if (!commuters.TryGetValue(inCommuter.CommuterId, out commuter))
                return new MethodResponse(false, ResponseCodes.InvalidInputParameter, ResponseMessages.CommuterDoesNotExists);

            commuter.HomeCoordinate = homeCoordinate;
            return new MethodResponse(true, ResponseCodes.SuccessDoNotRetry);
        }

        public static Commuter GetCommuter(Guid commuterId)
        {
            if (commuters == null || commuters.Count == 0)
                return null;
            return commuters[commuterId];
        }

        public static Commuter GetCommuter(string mediaId)
        {
            if (commuters == null || commuters.Count == 0)
                return null;
            foreach (var commuter in commuters)
            {
                if (commuter.Value.MediaId == mediaId)
                    return commuter.Value;
            }

            return null;
        }
    }
}
