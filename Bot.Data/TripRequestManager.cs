using Bot.Common;
using Bot.Data.DatastoreInterface;
using Bot.Data.EfModels;
using Bot.Data.Models;
using Bot.Extensions;
using Bot.Logger;
using Bot.Models.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Data
{
    public class TripRequestManager
    {
        private IStoreTripRequests tripReuestsStore;
        private IStoreCommuters commutersStore;

        private static TripRequestManager _tripRequestManager;
        private bool IsUpdateInProcess = false;

        private TripRequestManager()
        {
            var ctx = new DatabaseContext();
            tripReuestsStore = ctx;
            commutersStore = ctx;
        }

        public static TripRequestManager Instance
        {
            get
            {
                if (_tripRequestManager == null)
                    _tripRequestManager = new TripRequestManager();
                return _tripRequestManager;
            }
        }

        private IDictionary<Coordinate, IList<TripRequest>> Commuters =
            new Dictionary<Coordinate, IList<TripRequest>>();
        private IDictionary<Coordinate, IList<TripRequest>> Poolers =
            new Dictionary<Coordinate, IList<TripRequest>>();

        public MethodResponse AddTripRequest(TripRequest request)
        {
            new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.AddTripRequestStarted, request)
                .Debug();

            IDictionary<Coordinate, IList<TripRequest>> requestList;
            Coordinate origin;

            if (request.GoingHow == GoingHow.Drive)
                requestList = Commuters;
            else if (request.GoingHow == GoingHow.Pool)
                requestList = Poolers;
            else
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.InvalidArguments, request)
                {
                    Message = "GoingHow is not valid : " + request.GoingHow
                }.Error();
                return new MethodResponse(false, ResponseCodes.InvalidInputParameter);
            }

            if (request.GoingTo == GoingTo.Office)
                origin = PoolingMath.GetKeyPoint(request.Commuter.HomeCoordinate);
            else if (request.GoingTo == GoingTo.Home)
                origin = PoolingMath.GetKeyPoint(request.Commuter.OfficeCoordinate);
            else
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.InvalidArguments, request)
                {
                    Message = "GoingTo is not valid : " + request.GoingTo
                }.Error();
                return new MethodResponse(false, ResponseCodes.InvalidInputParameter);
            }

            request.Status = RequestStatus.Initialized;

            if (!requestList.Keys.Contains(origin))
                requestList.Add(origin, new List<TripRequest>());

            var result = tripReuestsStore.AddTripRequestsAsync(request.OperationId, request.FlowId, new List<TripRequest> { request }).Result;
            if (result)
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.AddingTripRequestToState, request)
                    .Debug();
                requestList[origin].Add(request);
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.AddedRequestToState, request)
                    .Debug();
                return new MethodResponse(true, ResponseCodes.SuccessDoNotRetry);
            }

            new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.FailedToRequestToState, request)
                .Error();
            return new MethodResponse(false, ResponseCodes.InternalProcessErrorDoNotRetry);
        }

        public bool UpdateRequestStatus(TripRequest request, RequestStatus status)
        {
            //todo: fix this mech
            while (IsUpdateInProcess) ;

            new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.UpdatingRequestStatus, request)
            {
                Message = "Updating status to : " + status
            }.Debug();

            IsUpdateInProcess = true;
            if (!UpdateRequestStatusThreadSafe(request, status))
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.UpdateRequestStatusFailed, request)
                {
                    Message = "Updating status to : " + status
                }.Error();
                IsUpdateInProcess = false;
                return false;
            }
            IsUpdateInProcess = false;

            new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.UpdatedRequestStatus, request)
            {
                Message = "Updating status to : " + status
            }.Debug();

            return true;
        }

        private bool UpdateRequestStatusThreadSafe(TripRequest request, RequestStatus status)
        {
            IDictionary<Coordinate, IList<TripRequest>> requestList;
            Coordinate origin;

            if (request.GoingHow == GoingHow.Drive)
                requestList = Commuters;
            else if (request.GoingHow == GoingHow.Pool)
                requestList = Poolers;
            else
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.RequestGoingHowIsInvalid, request)
                {
                    Message = "Updating status to : " + status
                }.Error();
                return false;
            }

            if (request.GoingTo == GoingTo.Office)
                origin = PoolingMath.GetKeyPoint(request.Commuter.HomeCoordinate);
            else if (request.GoingTo == GoingTo.Home)
                origin = PoolingMath.GetKeyPoint(request.Commuter.OfficeCoordinate);
            else
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.RequestGoingToIsInvalid, request)
                {
                    Message = "Updating status to : " + status
                }.Error();
                return false;
            }

            if (!requestList.Keys.Contains(origin))
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.RequestNodeDoesNotExistsInState, request)
                {
                    Message = "RequestList Keys : " + requestList.Keys.ToJsonString()
                }.Error();
                return false;
            }

            request.Status = status;
            var tripRequestToUpdate = requestList[origin].
                FirstOrDefault(r => r.Commuter.CommuterId == request.Commuter.CommuterId);

            if (tripRequestToUpdate == null)
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.RequestDoesNotExistsInState, request)
                {
                    Message = "RequestList : " + requestList[origin].ToJsonString()
                }.Error();
                return false;
            }

            if (!tripReuestsStore.UpdateTripRequestAsync(new List<TripRequest> { request }).Result)
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.RequestDidNotUpdateInStore, tripRequestToUpdate)
                    .Error();
                return false;
            }

            tripRequestToUpdate.Status = status;
            if (status == RequestStatus.InTrip)
            {
                request.Commuter.Status = CommuterStatus.InTrip;
                if (commutersStore.UpdateCommutersAsync(Guid.NewGuid(), Guid.NewGuid(),new List<Commuter> { request.Commuter }).Result)
                {
                    tripRequestToUpdate.Commuter.Status = CommuterStatus.InTrip;
                }
            }
            else
            {
                request.Commuter.Status = CommuterStatus.InTrip;
                if (commutersStore.UpdateCommutersAsync(Guid.NewGuid(), Guid.NewGuid(), new List<Commuter> { request.Commuter }).Result)
                {
                    tripRequestToUpdate.Commuter.Status = CommuterStatus.InProcess;
                }
            }

            new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.RequestStatusUpdated, tripRequestToUpdate).Error();
            return true;
        }

        public MethodResponse<IList<TripRequest>> GetWaitingCommuterRequests()
        {
            //after caching
            //var waitingCommuters = new List<TripRequest>();
            //foreach (var commuterRequestList in Commuters)
            //{
            //    waitingCommuters = commuterRequestList.Value
            //        .Where(r => r.Status == RequestStatus.Initialized || r.Status == RequestStatus.Waiting)
            //        .ToList();
            //}

            var requests = tripReuestsStore.GetWaitingTripRequests().Result;
            return new MethodResponse<IList<TripRequest>>(requests);
        }

        public IDictionary<Coordinate, IList<TripRequest>> GetAllPoolerRequests()
        {
            return Poolers;
        }

        public IEnumerable<TripRequest> GetInitializedPoolerRequestsForCoordinate(Coordinate coordinate)
        {
            var eligiblePoolerRequests = new List<TripRequest>();

            IList<TripRequest> poolerRequests;
            if (!Poolers.TryGetValue(coordinate, out poolerRequests))
            {
                return eligiblePoolerRequests;
            }

            foreach (var poolerRequest in poolerRequests)
            {
                if (poolerRequest.Status == RequestStatus.Initialized)
                    eligiblePoolerRequests.Add(poolerRequest);
            }

            return eligiblePoolerRequests;
        }

        public void ClearAllRequests()
        {
            Commuters = new Dictionary<Coordinate, IList<TripRequest>>();
            Poolers = new Dictionary<Coordinate, IList<TripRequest>>();
        }
    }
}
