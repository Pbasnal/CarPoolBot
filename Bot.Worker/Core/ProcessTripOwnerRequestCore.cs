using Bot.Common;
using Bot.Data;
using Bot.Data.Models;
using Bot.External;
using Bot.Logger;
using Bot.MessagingFramework;
using Bot.Models.Internal;
using Bot.Worker.Messages;
using Bot.Worker.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Worker.Core
{
    public class ProcessTripOwnerRequestCore
    {
        private EngineState _engineState;
        public ProcessTripOwnerRequestCore()
        {
            _engineState = EngineState.Instance;
        }
        public EngineState EngineState
        {
            get
            {
                return _engineState?.Clone();
            }
        }

        // todo:should return the data for which poolingengine could send requests messages to the customers.
        public MethodResponse ProcessRequests(ProcessTripOwnerRequestMessage message)
        {
            var methodResponse = AddCommuterRequestsToState(message.TripOwnerRequest);
            if (!methodResponse.IsSuccess)
            {
                new BotLogger<ProcessTripOwnerRequestMessage>(message.OperationId, message.MessageId, EventCodes.UnableToAddCommuterRequestToState, message)
                    .Error();
                return methodResponse;
            }
            var process = _engineState.CommuterRequestProcessTable.FirstOrDefault(x => x.Key == message.TripOwnerRequest.Commuter.CommuterId);

            new BotLogger<ProcessTripOwnerRequestMessage>(message.OperationId, message.MessageId, EventCodes.AddingPoolersToState, message)
                .Debug();
            var response = CommonEngineCore.AddPoolersRequestsToState(process.Value);

            if (!response.IsSuccess)
            {
                new BotLogger<ProcessTripOwnerRequestMessage>(message.OperationId, message.MessageId, EventCodes.UnableToAddPoolersToState, message)
                    .Error();
            }

            PublishRequestOwnerMessage(response.ResultData.TripOwnerRequest.TripRequest, response.ResponseCode);

            return new MethodResponse { IsSuccess = true };
        }

        private MethodResponse AddCommuterRequestsToState(TripRequest tripOwnerRequest)
        {
            var maps = new Maps();
            IList<Coordinate> route = maps.GetRoute(tripOwnerRequest);

            if (route == null || route.Count == 0)
            {
                new BotLogger<TripRequest>(tripOwnerRequest.OperationId, tripOwnerRequest.FlowId, EventCodes.UnableToGetRouteForTheTrip, tripOwnerRequest)
                    .Error();
                return new MethodResponse(false, ResponseCodes.InternalProcessErrorRetry, ResponseMessages.RouteNotFound);
            }
            new BotLogger<IList<Coordinate>>(tripOwnerRequest.OperationId, tripOwnerRequest.FlowId, EventCodes.GotRouteForTheRequest, route)
                .Debug();

            if (!TripRequestManager.Instance.UpdateRequestStatus(tripOwnerRequest, RequestStatus.InProcess))
            {
                new BotLogger<TripRequest>(tripOwnerRequest.OperationId, tripOwnerRequest.FlowId, EventCodes.UnableToUpdateRequestStatus, tripOwnerRequest)
                    .Error();
                return new MethodResponse(false, ResponseCodes.InternalProcessErrorRetry, ResponseMessages.FailedToUpdateState);
            }
            if (!_engineState.AddCommuterToState(tripOwnerRequest, route).IsSuccess)
            {
                new BotLogger<TripRequest>(tripOwnerRequest.OperationId, tripOwnerRequest.FlowId, EventCodes.UnableToAddCommuterToState, tripOwnerRequest)
                    .Error();
                return new MethodResponse(false, ResponseCodes.InternalProcessErrorRetry, ResponseMessages.FailedToAddToState);
            }

            MessageBus.Instance.Publish(new VehicleOwnerAddedToStateMessage(tripOwnerRequest.OperationId, tripOwnerRequest.FlowId)
            {
                Route = new Data.Route(route.ToList()),
                TripRequest = tripOwnerRequest
            });

            new BotLogger<TripRequest>(tripOwnerRequest.OperationId, tripOwnerRequest.FlowId, EventCodes.AddedCommuterToState, tripOwnerRequest)
                .Debug();

            return new MethodResponse(true, ResponseCodes.SuccessDoNotRetry);
        }

        private void PublishRequestOwnerMessage(TripRequest request, int status)
        {
            CommuterRequestProcessModel commuterRequestProcess;
            if (!_engineState.CommuterRequestProcessTable.TryGetValue(request.Commuter.CommuterId, out commuterRequestProcess))
            {
                new BotLogger<TripRequest>(request.OperationId, request.FlowId, EventCodes.ProcessModelDoesNotExists, request)
                    .Error();
                return;
            }

            var waitingPoolers = commuterRequestProcess.PoolerRequests
                .Where(x => x.Status == TripRequestStatus.Waiting).ToList();
            int numberOfPoolersToRequest =
                (request.Commuter.Vehicle.RemainingSeats > waitingPoolers.Count) ?
                waitingPoolers.Count : request.Commuter.Vehicle.RemainingSeats;

            var poolersToRequest = waitingPoolers.GetRange(0, numberOfPoolersToRequest);

            new BotLogger<CommuterRequestProcessModel>(request.OperationId, request.FlowId, EventCodes.GotPoolersToRequest, commuterRequestProcess)
                    .Debug();

            foreach (var poolRequest in poolersToRequest)
            {
                var tripInRequest = commuterRequestProcess.PoolerRequests
                    .First(x => x.TripRequest.Commuter.CommuterId == poolRequest.TripRequest.Commuter.CommuterId);
                tripInRequest.Status = TripRequestStatus.Requested;

                new BotLogger<TripRequestInProcess>(request.OperationId, request.FlowId, EventCodes.UpdatingRequestStatusOfPoolerRequest, tripInRequest)
                    .Debug();

                var isUpdateSuccess = TripRequestManager.Instance.UpdateRequestStatus(poolRequest.TripRequest, RequestStatus.InProcess);

                if (!isUpdateSuccess)
                {
                    new BotLogger<TripRequestInProcess>(request.OperationId, request.FlowId, EventCodes.UpdatRequestStatusOfPoolerRequestFailed, tripInRequest)
                        .Debug();
                    //todo: what to do here? poolers need to be reversed. transaction kind of stuff
                    //break;
                }
                new BotLogger<TripRequestInProcess>(request.OperationId, request.FlowId, EventCodes.UpdatedRequestStatusOfPoolerRequest, tripInRequest)
                    .Debug();
            }
            MessageBus.Instance.Publish(new ReqestOwnerToAcceptPoolersMessage(request.OperationId, request.FlowId)
            {
                PoolersToRequestFor = poolersToRequest,
                Status = status,
                OwnerRequest = commuterRequestProcess.TripOwnerRequest.TripRequest
            });
        }
    }
}
