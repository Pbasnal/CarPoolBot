using Bot.Data.DatastoreInterface;
using Bot.Data.EfModels;
using Bot.Data.Models;
using Bot.Models.Internal;
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
            IDictionary<Coordinate, IList<TripRequest>> requestList;
            Coordinate origin;

            if (request.GoingHow == GoingHow.Drive)
                requestList = Commuters;
            else if (request.GoingHow == GoingHow.Pool)
                requestList = Poolers;
            else
                return new MethodResponse(false, ResponseCodes.InvalidInputParameter);

            if (request.GoingTo == GoingTo.Office)
                origin = PoolingMath.GetKeyPoint(request.Commuter.HomeCoordinate);
            else if (request.GoingTo == GoingTo.Home)
                origin = PoolingMath.GetKeyPoint(request.Commuter.OfficeCoordinate);
            else
                return new MethodResponse(false, ResponseCodes.InvalidInputParameter); ;

            if (!requestList.Keys.Contains(origin))
                requestList.Add(origin, new List<TripRequest>());
            request.Status = RequestStatus.Initialized;

            var result = tripReuestsStore.AddTripRequestsAsync(new List<TripRequest> { request }).Result;
            if (result)
            {
                requestList[origin].Add(request);
                return new MethodResponse(true, ResponseCodes.SuccessDoNotRetry);
            }
            return new MethodResponse(false, ResponseCodes.InternalProcessError);
        }

        public bool UpdateRequestStatus(TripRequest request, RequestStatus status)
        {
            while (IsUpdateInProcess) ;

            IsUpdateInProcess = true;
            if (!UpdateRequestStatusThreadSafe(request, status))
            {
                IsUpdateInProcess = false;
                return false;
            }
            IsUpdateInProcess = false;
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
                return false;

            if (request.GoingTo == GoingTo.Office)
                origin = PoolingMath.GetKeyPoint(request.Commuter.HomeCoordinate);
            else if (request.GoingTo == GoingTo.Home)
                origin = PoolingMath.GetKeyPoint(request.Commuter.OfficeCoordinate);
            else
                return false;

            if (!requestList.Keys.Contains(origin))
                return false;

            request.Status = status;
            var tripRequestToUpdate = requestList[origin].
                FirstOrDefault(r => r.Commuter.CommuterId == request.Commuter.CommuterId);

            if (tripRequestToUpdate == null)
                return false;

            if (!tripReuestsStore.UpdateTripRequestAsync(new List<TripRequest> { request }).Result)
                return false;

            tripRequestToUpdate.Status = status;
            if (status == RequestStatus.InTrip)
            {
                request.Commuter.Status = CommuterStatus.InTrip;
                if (commutersStore.UpdateCommutersAsync(new List<Commuter> { request.Commuter }).Result)
                {
                    tripRequestToUpdate.Commuter.Status = CommuterStatus.InTrip;
                }
            }
            else
            {
                request.Commuter.Status = CommuterStatus.InTrip;
                if (commutersStore.UpdateCommutersAsync(new List<Commuter> { request.Commuter }).Result)
                {
                    tripRequestToUpdate.Commuter.Status = CommuterStatus.InProcess;
                }
            }
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
