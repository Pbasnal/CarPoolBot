using Bot.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Data
{
    public class TripRequestManager
    {
        private static bool IsUpdateInProcess = false;

        private TripRequestManager()
        {
        }

        private static IDictionary<Coordinate, IList<TripRequest>> Commuters =
            new Dictionary<Coordinate, IList<TripRequest>>();
        private static IDictionary<Coordinate, IList<TripRequest>> Poolers =
            new Dictionary<Coordinate, IList<TripRequest>>();

        public static void AddTripRequest(TripRequest request)
        {
            IDictionary<Coordinate, IList<TripRequest>> requestList;
            Coordinate origin;

            if (request.GoingHow == GoingHow.Drive)
                requestList = Commuters;
            else if (request.GoingHow == GoingHow.Pool)
                requestList = Poolers;
            else
                return;

            if (request.GoingTo == GoingTo.Office)
                origin = PoolingMath.GetKeyPoint(request.Commuter.HomeCoordinate);
            else if (request.GoingTo == GoingTo.Home)
                origin = PoolingMath.GetKeyPoint(request.Commuter.OfficeCoordinate);
            else
                return;

            if (!requestList.Keys.Contains(origin))
                requestList.Add(origin, new List<TripRequest>());
            request.Status = RequestStatus.Initialized;
            requestList[origin].Add(request);
        }

        public static void UpdateRequestStatus(TripRequest request, RequestStatus status)
        {
            while (IsUpdateInProcess) ;

            IsUpdateInProcess = true;
            UpdateRequestStatusThreadSafe(request, status);
            IsUpdateInProcess = false;
        }

        private static void UpdateRequestStatusThreadSafe(TripRequest request, RequestStatus status)
        {
            IDictionary<Coordinate, IList<TripRequest>> requestList;
            Coordinate origin;

            if (request.GoingHow == GoingHow.Drive)
                requestList = Commuters;
            else if (request.GoingHow == GoingHow.Pool)
                requestList = Poolers;
            else
                return;

            if (request.GoingTo == GoingTo.Office)
                origin = PoolingMath.GetKeyPoint(request.Commuter.HomeCoordinate);
            else if (request.GoingTo == GoingTo.Home)
                origin = PoolingMath.GetKeyPoint(request.Commuter.OfficeCoordinate);
            else
                return;

            if (!requestList.Keys.Contains(origin))
                return;

            request.Status = status;
            var tripRequestToUpdate = requestList[origin].
                FirstOrDefault(r => r.Commuter.CommuterId == request.Commuter.CommuterId);

            if (tripRequestToUpdate == null)
                return;
            tripRequestToUpdate.Status = status;
            if (status == RequestStatus.InTrip)
                tripRequestToUpdate.Commuter.Status = CommuterStatus.InTrip;
            else
                tripRequestToUpdate.Commuter.Status = CommuterStatus.InProcess;
        }

        public static IDictionary<Coordinate, IList<TripRequest>> GetAllCommuterRequests()
        {
            return Commuters;
        }

        public static IList<TripRequest> GetWaitingCommuterRequests()
        {
            var waitingCommuters = new List<TripRequest>();

            foreach (var commuterRequestList in Commuters)
            {
                waitingCommuters = commuterRequestList.Value
                    .Where(r => r.Status == RequestStatus.Initialized || r.Status == RequestStatus.Waiting)
                    .ToList();
                //foreach (var request in commuterRequestList.Value)
                //{
                //    if (request.Status != RequestStatus.Initialized && request.Status != RequestStatus.Waiting)
                //        continue;
                //    waitingCommuters.Add(request);
                //}
            }
            return waitingCommuters;
        }

        public static IDictionary<Coordinate, IList<TripRequest>> GetAllPoolerRequests()
        {
            return Poolers;
        }

        public static IEnumerable<TripRequest> GetInitializedPoolerRequestsForCoordinate(Coordinate coordinate)
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

        public static void ClearAllRequests()
        {
            Commuters = new Dictionary<Coordinate, IList<TripRequest>>();
            Poolers = new Dictionary<Coordinate, IList<TripRequest>>();
        }
    }
}
