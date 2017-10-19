using Bot.Data;
using Bot.Models.Internal;
using Bot.Worker.Core;
using Bot.Worker.Models;
using Bot.Worker.Requests;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Worker
{
    //todo: handle callbacks 
    public class PoolingEngine
    {
        private static PoolingEngine _instance;
        private Queue<ProcessNewRequest> _newProcessRequests;

        private ManualResetEvent _newRequestsHandle;
        private ManualResetEvent _addPoolersToTripHandle;

        private EngineCore _core;

        public EngineState State
        {
            get
            {
                return _core.EngineState;
            }
        }

        private PoolingEngine()
        {
            _newProcessRequests = new Queue<ProcessNewRequest>();
            _core = new EngineCore();
        }

        public static PoolingEngine Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PoolingEngine();
                return _instance;
            }
        }

        // maybe a separate workflow for handling delegates and interactions
        // seems like actor model would be a better fit for this!!
        public delegate void RequestCompleteCallbackDelegate(TripRequest request);

        public static RequestCompleteCallbackDelegate RequestCompleteCallback;

        public async Task QueuePoolingRequest()
        {
            await Task.Factory.StartNew(() =>
            {
                _core.ProcessRequests();
            });
        }

        public async void AddPoolersToTrip(TripRequest commuterRequest, int[] poolerIndices)
        {
            await Task.Factory.StartNew(() =>
            {
                var methodResponse = _core.AddPoolersToTrip(commuterRequest, poolerIndices);
                if (methodResponse.ResponseInfo.ResponseCode != ResponseCodes.TripDidNotStart)
                    methodResponse = _core.StartTrip(commuterRequest);
                if (methodResponse.ResponseInfo.ResponseCode == ResponseCodes.TripStarted)
                    RequestCompleteCallback(commuterRequest);
            });
        }

        public void EndTheTrip(TripRequest request)
        {
            _core.CompleteCommuterRequest(request);
        }

        public IList<TripRequest> GetPoolersToRequestForTrip(TripRequest request)
        {
            return _core.GetPoolersToRequestForTrip(request);
        }

        public IList<Commuter> GetPoolersInTrip(TripRequest request)
        {
            return _core.GetPoolersInTrip(request);
        }

        public Models.Route GetCommuterRoute(TripRequest request)
        {
            return _core.GetTripRoute(request);
        }

        public void SetRequestCallBack(RequestCompleteCallbackDelegate callbackMethod)
        {
            RequestCompleteCallback = callbackMethod;
        }
    }
}
