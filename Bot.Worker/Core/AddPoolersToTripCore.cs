using Bot.Common;
using Bot.Data;
using Bot.Data.Models;
using Bot.Logger;
using Bot.MessagingFramework;
using Bot.Models.Internal;
using Bot.Worker.Messages;
using Bot.Worker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Bot.Data.DataManagers;

namespace Bot.Worker.Core
{
    public class AddPoolersToTripCore
    {
        private EngineState _engineState;
        
        public AddPoolersToTripCore()
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

        public MethodResponse<CommuterRequestProcessModel> AddPoolersToTrip(AddPoolersToTripMessage message)
        {
            CommuterRequestProcessModel commuterRequestProcess;
            if (!_engineState.CommuterRequestProcessTable.TryGetValue(message.TripRequest.Commuter.CommuterId, out commuterRequestProcess))
            {
                new BotLogger<AddPoolersToTripMessage>(message.OperationId, message.MessageId, EventCodes.ProcessModelDoesNotExists, message)
                    .Error();
                return new MethodResponse<CommuterRequestProcessModel>(null);
            }

            var addPoolersResponse = AddPoolersToTripAndUpdatePoolerStatus(message, commuterRequestProcess);
            if (!addPoolersResponse.IsSuccess)
            {
                new BotLogger<AddPoolersToTripMessage>(message.OperationId, message.MessageId, EventCodes.FailedToAddPoolers, message)
                    .Error();
                return new MethodResponse<CommuterRequestProcessModel>(null);
            }
            IList<TripRequest> acceptedPoolerRequests = addPoolersResponse.ResultData;

            RemoveAcceptedPoolersFromProcessTable(commuterRequestProcess, acceptedPoolerRequests);
            new BotLogger<CommuterRequestProcessModel>(message.OperationId, message.MessageId, EventCodes.AcceptedPoolersRemovedFromProcessModel, commuterRequestProcess)
                .Debug();

            SetRejectedPoolersStateToInitialized(message.MessageId, commuterRequestProcess);
            new BotLogger<AddPoolersToTripMessage>(message.OperationId, message.MessageId, EventCodes.RejectedPoolersSetToInitialized, message)
                .Debug();

            var methodResponse = new MethodResponse<CommuterRequestProcessModel>
            {
                IsSuccess = true,
                ResponseCode = ResponseCodes.SuccessDoNotRetry
            };
            if (message.TripRequest.Commuter.Vehicle.RemainingSeats > 0 && message.SearchForMorePoolers)
            {
                new BotLogger<AddPoolersToTripMessage>(message.OperationId, message.MessageId, EventCodes.AddingMorePoolersToState, message)
                    .Debug();
                methodResponse = CommonEngineCore.AddPoolersRequestsToState(message.MessageId, commuterRequestProcess);
            }

            if (methodResponse.ResponseCode == ResponseCodes.SuccessDoNotRetry)
            {
                TripRequestManager.Instance.UpdateRequestStatus(message.MessageId, message.TripRequest, RequestStatus.InTrip);
                TripManager.Instance.StartNewTrip(message.MessageId, commuterRequestProcess.Trip);

                MessageBus.Instance.Publish(new TripStartedMessage(message.OperationId, message.MessageId)
                {
                    Trip = commuterRequestProcess.Trip,
                    Route = commuterRequestProcess.CompleteRoute
                });
                new BotLogger<AddPoolersToTripMessage>(message.OperationId, message.MessageId, EventCodes.MarkingRequestStateInTrip, message)
                    .Debug();
                
                return methodResponse;
            }
            if (methodResponse.ResponseCode == ResponseCodes.SuccessCanRetry)
            {
                MessageBus.Instance.Publish(new TripDidNotStartedMessage(message.OperationId, message.MessageId)
                {
                    Trip = commuterRequestProcess.Trip,
                    Route = commuterRequestProcess.CompleteRoute
                });
            }
            return methodResponse;
        }

        private MethodResponse<IList<TripRequest>> AddPoolersToTripAndUpdatePoolerStatus(AddPoolersToTripMessage message, CommuterRequestProcessModel commuterRequestProcess)
        {
            Tuple<AddPoolersToTripMessage, CommuterRequestProcessModel> logObject = new Tuple<AddPoolersToTripMessage, CommuterRequestProcessModel>(message, commuterRequestProcess);
            new BotLogger<Tuple<AddPoolersToTripMessage, CommuterRequestProcessModel>>
                (message.OperationId, message.MessageId, EventCodes.AddingPoolersToTripAndUpdatePoolerStatus, logObject).Debug();

            var acceptedPoolerRequests = new List<TripRequest>();
            foreach (var index in message.PoolerIndices)
            {
                acceptedPoolerRequests.Add(commuterRequestProcess.PoolerRequests[index].TripRequest);
                commuterRequestProcess.Trip.AddPassenger(commuterRequestProcess.PoolerRequests[index].TripRequest.Commuter);
                if (!TripRequestManager.Instance.UpdateRequestStatus(message.MessageId, commuterRequestProcess.PoolerRequests[index].TripRequest, RequestStatus.InTrip))
                {
                    new BotLogger<Tuple<AddPoolersToTripMessage, CommuterRequestProcessModel>>
                    (message.OperationId, message.MessageId, EventCodes.UpdatingRequestStatusFailed, logObject).Error();

                    return new MethodResponse<IList<TripRequest>>(false, ResponseCodes.InternalProcessErrorDoNotRetry);
                }

                MessageBus.Instance.Publish(new PoolerAddedToTripMessage(message.OperationId, message.MessageId)
                {
                    Trip = commuterRequestProcess.Trip,
                    Pooler = commuterRequestProcess.PoolerRequests[index].TripRequest.Commuter
                });
            }

            new BotLogger<Tuple<AddPoolersToTripMessage, CommuterRequestProcessModel>>
                (message.OperationId, message.MessageId, EventCodes.UpdatedRequestStatus, logObject).Debug();

            return new MethodResponse<IList<TripRequest>>(acceptedPoolerRequests);
        }

        private void SetRejectedPoolersStateToInitialized(Guid flowId, CommuterRequestProcessModel commuterRequestProcess)
        {
            foreach (var poolRequest in commuterRequestProcess
                .PoolerRequests.Where(x => x.Status == TripRequestStatus.Requested))
            {
                TripRequestManager.Instance.UpdateRequestStatus(flowId, poolRequest.TripRequest, RequestStatus.Initialized);
                commuterRequestProcess.PoolerRequests.Remove(poolRequest);
            }
        }

        private void RemoveAcceptedPoolersFromProcessTable(CommuterRequestProcessModel commuterRequestProcess, IList<TripRequest> acceptedPoolerRequests)
        {
            foreach (var request in acceptedPoolerRequests)
            {
                var tripInRequest = commuterRequestProcess.PoolerRequests
                    .First(x => x.TripRequest.Commuter.CommuterId == request.Commuter.CommuterId);
                commuterRequestProcess.PoolerRequests.Remove(tripInRequest);
            }
        }

    }
}
