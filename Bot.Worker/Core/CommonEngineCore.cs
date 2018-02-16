using Bot.Data;
using Bot.Data.Models;
using Bot.Models.Internal;
using Bot.Worker.Models;
using System.Collections.Generic;
using System.Linq;
using Bot.Data.DataManagers;
using System;

namespace Bot.Worker.Core
{
    internal class CommonEngineCore
    {
        static private EngineState _engineState = EngineState.Instance;
        
        static public MethodResponse<CommuterRequestProcessModel> AddPoolersRequestsToState(Guid flowId, CommuterRequestProcessModel commuterRequestProcess)
        {
            int numberOfPoolersNeeded = commuterRequestProcess.Trip.Owner.Vehicle.RemainingSeats
                - commuterRequestProcess.PoolerRequests.Where(x => x.Status == TripRequestStatus.Waiting).Count();

            while (numberOfPoolersNeeded > 0 && !commuterRequestProcess.LastNodeReached)
            {
                var currentNode = commuterRequestProcess.CurrentNode;
                IList<TripRequest> poolersForCurrentNode = TripRequestManager.Instance.GetInitializedPoolerRequestsForCoordinate(currentNode).ToList();

                foreach (var poolRequest in poolersForCurrentNode)
                {
                    if (commuterRequestProcess.PoolerRequests.Any(x => x.TripRequest.Commuter.CommuterId == poolRequest.Commuter.CommuterId))
                        continue;
                    var tripRequestInProcess = new TripRequestInProcess { TripRequest = poolRequest, Status = TripRequestStatus.Waiting };
                    commuterRequestProcess.PoolerRequests.Add(tripRequestInProcess);
                    _engineState.PoolerCommuterMapping.Add(poolRequest.Commuter.CommuterId,
                        commuterRequestProcess.Trip.Owner.CommuterId);
                    TripRequestManager.Instance.UpdateRequestStatus(flowId, poolRequest, RequestStatus.Waiting);
                    numberOfPoolersNeeded--;

                    if (numberOfPoolersNeeded <= 0)
                        break;
                }
            }

            var methodResponse = new MethodResponse<CommuterRequestProcessModel>(commuterRequestProcess);
            if (numberOfPoolersNeeded == 0 || (numberOfPoolersNeeded > 0 && commuterRequestProcess.LastNodeReached))
            {
                methodResponse.ResponseCode = ResponseCodes.SuccessDoNotRetry;
            }
            else if (numberOfPoolersNeeded > 0 && !commuterRequestProcess.LastNodeReached)
            {
                methodResponse.ResponseCode = ResponseCodes.SuccessCanRetry;
            }
            //PublishRequestOwnerMessage(commuterRequestProcess.TripOwnerRequest.TripRequest, methodResponse.ResponseCode);

            methodResponse.IsSuccess = true;
            return methodResponse;
        }
    }
}
