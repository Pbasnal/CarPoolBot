using Bot.Common;
using Bot.Data.DatastoreInterface;
using Bot.Data.EfModels;
using Bot.Data.Models;
using Bot.Logger;
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

            var result = CommutersStore.AddCommutersAsync(commuter.OperationId, commuter.FlowId, new List<Commuter> { commuter }).Result;

            if (result)
            {
                commuters.Add(commuter.CommuterId, commuter);
                return new MethodResponse<Commuter>(commuter);
            }

            return new MethodResponse<Commuter>(null);
        }

        public MethodResponse<Commuter> AddCommuter(Commuter commuter)
        {
            new BotLogger<Commuter>(commuter.OperationId, commuter.FlowId, EventCodes.AddingCommuterToState, commuter)
                .Debug();

            var result = CommutersStore.AddCommutersAsync(commuter.OperationId, commuter.FlowId, new List<Commuter> { commuter }).Result;

            if (result)
            {
                commuters.Add(commuter.CommuterId, commuter);

                new BotLogger<Commuter>(commuter.OperationId, commuter.FlowId, EventCodes.CommuterAddedToState, commuter)
                .Debug();

                return new MethodResponse<Commuter>(commuter);
            }

            new BotLogger<Commuter>(commuter.OperationId, commuter.FlowId, EventCodes.CommuterNotAddedToState, commuter)
                .Error();

            return new MethodResponse<Commuter>(null);
        }

        public MethodResponse<Commuter> AddOfficeOfCommuter(Commuter inCommuter, Coordinate officeCoordinate)
        {
            Commuter commuter;

            var methodParameterLogObject = new Tuple<Commuter, Coordinate>(inCommuter, officeCoordinate);
            new BotLogger<Tuple<Commuter, Coordinate>>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.AddOfficeOfCommuterToStateAndDb, methodParameterLogObject)
                .Debug();

            if (!commuters.TryGetValue(inCommuter.CommuterId, out commuter))
            {
                new BotLogger<Commuter>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.UserDoesNotExists, commuter)
                .Error();
                return new MethodResponse<Commuter>(false, ResponseCodes.InvalidInputParameter, ResponseMessages.CommuterDoesNotExists);
            }

            //improve this
            var oldValue = commuter.OfficeCoordinate;
            commuter.OfficeCoordinate = officeCoordinate;

            var result = CommutersStore.UpdateCommutersAsync(commuter.OperationId, commuter.FlowId, new List<Commuter> { commuter }).Result;

            if (result)
            {
                new BotLogger<Commuter>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.OfficeAddedToDb, commuter)
                .Debug();

                commuters.Add(commuter.CommuterId, commuter);

                new BotLogger<Commuter>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.OfficeAddedToState, commuter)
                .Debug();
                return new MethodResponse<Commuter>(commuter);
            }
            
            //revertiung if db didn't get updated
            new BotLogger<Tuple<Commuter, Coordinate>>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.ErrorWhileAddingOfficeLocationToDbRevertingChanges, methodParameterLogObject)
                .Error();
            commuter.OfficeCoordinate = oldValue;

            return new MethodResponse<Commuter>(null);
        }

        public MethodResponse<Commuter> AddHouseOfCommuter(Commuter inCommuter, Coordinate homeCoordinate)
        {
            Commuter commuter;

            var methodParameterLogObject = new Tuple<Commuter, Coordinate>(inCommuter, homeCoordinate);
            new BotLogger<Tuple<Commuter, Coordinate>>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.AddHomeOfCommuterToStateAndDb, methodParameterLogObject)
                .Debug();

            if (!commuters.TryGetValue(inCommuter.CommuterId, out commuter))
            {
                new BotLogger<Commuter>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.UserDoesNotExists, commuter)
                .Error();
                return new MethodResponse<Commuter>(false, ResponseCodes.InvalidInputParameter, ResponseMessages.CommuterDoesNotExists);
            }
            //improve this
            var oldValue = commuter.HomeCoordinate;
            commuter.HomeCoordinate = homeCoordinate;

            var result = CommutersStore.UpdateCommutersAsync(commuter.OperationId, commuter.FlowId, new List<Commuter> { commuter }).Result;

            if (result)
            {
                new BotLogger<Commuter>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.HomeAddedToDb, commuter)
                .Debug();

                commuters.Add(commuter.CommuterId, commuter);

                new BotLogger<Commuter>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.HomeAddedToState, commuter)
                .Debug();

                return new MethodResponse<Commuter>(commuter);
            }
            //revertiung if db didn't get updated
            new BotLogger<Tuple<Commuter, Coordinate>>(inCommuter.OperationId, inCommuter.FlowId, EventCodes.ErrorWhileAddingHomeLocationToDbRevertingChanges, methodParameterLogObject)
                .Error();

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
