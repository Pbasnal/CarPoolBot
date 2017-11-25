using Bot.Data.DatastoreInterface;
using Bot.Data.EfModels;
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
        private static CommuterManager _commuterManager;
        private IDictionary<Guid, Commuter> commuters = new Dictionary<Guid, Commuter>();

        private IStoreCommuters CommutersStore = new DatabaseContext();

        private CommuterManager()
        { }

        public static CommuterManager Instance
        {
            get
            {
                if (_commuterManager == null)
                    _commuterManager = new CommuterManager();
                return _commuterManager;
            }
            set
            { }
        }

        public IDictionary<Guid, Commuter> CommutersList { get; private set; }

        public MethodResponse<Commuter> AddMicrosotCommuter(Commuter commuter)
        {
            var office = new Coordinate();
            office.Latitude = 17.4318848;
            office.Longitude = 78.34318;

            commuter.OfficeCoordinate = office;

            var result = CommutersStore.AddCommutersAsync(new List<Commuter> { commuter }).Result;

            if (result)
            {
                commuters.Add(commuter.CommuterId, commuter);
                return new MethodResponse<Commuter>(commuter);
            }

            return new MethodResponse<Commuter>(null);
        }

        public MethodResponse<Commuter> AddCommuter(Commuter commuter)
        {
            var result = CommutersStore.AddCommutersAsync(new List<Commuter> { commuter }).Result;

            if (result)
            {
                commuters.Add(commuter.CommuterId, commuter);
                return new MethodResponse<Commuter>(commuter);
            }

            return new MethodResponse<Commuter>(null);
        }

        public MethodResponse<Commuter> AddOfficeOfCommuter(Commuter inCommuter, Coordinate officeCoordinate)
        {
            Commuter commuter;
            if (!commuters.TryGetValue(inCommuter.CommuterId, out commuter))
                return new MethodResponse<Commuter>(false, ResponseCodes.InvalidInputParameter, ResponseMessages.CommuterDoesNotExists);

            //improve this
            var oldValue = commuter.OfficeCoordinate;
            commuter.OfficeCoordinate = officeCoordinate;

            var result = CommutersStore.UpdateCommutersAsync(new List<Commuter> { commuter }).Result;

            if (result)
            {
                commuters.Add(commuter.CommuterId, commuter);
                return new MethodResponse<Commuter>(commuter);
            }
            //revertiung if db didn't get updated
            commuter.OfficeCoordinate = oldValue;
            return new MethodResponse<Commuter>(null);
        }

        public MethodResponse<Commuter> AddHouseOfCommuter(Commuter inCommuter, Coordinate homeCoordinate)
        {
            Commuter commuter;
            if (!commuters.TryGetValue(inCommuter.CommuterId, out commuter))
                return new MethodResponse<Commuter>(false, ResponseCodes.InvalidInputParameter, ResponseMessages.CommuterDoesNotExists);


            //improve this
            var oldValue = commuter.OfficeCoordinate;
            commuter.HomeCoordinate = homeCoordinate;

            var result = CommutersStore.UpdateCommutersAsync(new List<Commuter> { commuter }).Result;

            if (result)
            {
                commuters.Add(commuter.CommuterId, commuter);
                return new MethodResponse<Commuter>(commuter);
            }
            //revertiung if db didn't get updated
            commuter.HomeCoordinate = oldValue;
            return new MethodResponse<Commuter>(null);
        }

        public MethodResponse<Commuter> GetCommuter(Guid commuterId)
        {
            if (commuters == null || commuters.Count == 0)
                return new MethodResponse<Commuter>(null);
            // later to be used with cache
            //return commuters[commuterId];
            var resultCommuters = CommutersStore.GetCommuters(new List<Guid> { commuterId }).Result;
            if (resultCommuters.Count == 1)
            {
                return new MethodResponse<Commuter>(resultCommuters[0]);
            }

            return new MethodResponse<Commuter>(null);
        }

        public MethodResponse<Commuter> GetCommuter(string mediaId)
        {
            if (commuters == null || commuters.Count == 0)
                return new MethodResponse<Commuter>(null);

            var resultCommuters = CommutersStore.GetCommutersForMediaIds(new List<string> { mediaId }).Result;
            if (resultCommuters.Count == 1)
            {
                return new MethodResponse<Commuter>(resultCommuters[0]);
            }

            return new MethodResponse<Commuter>(null);
        }
    }
}
