namespace Bot.Common
{
    public class EventCodes
    {
        public const string AddingTripSqlDb = "AddingTripSqlDb";
        public const string WrongUserMessage = "WrongUserMessage";
        public const string UserDoesNotExists = "UserDoesNotExists";
        public const string InvalidArguments = "InvalidArguments";

        //Bot.Authentication
        public const string AuthenticationStartedForuser = "AuthenticationStartedForuser";


        //Bot.Welcome
        public const string WelcomeDialogInitiatedForUser = "WelcomeDialogInitiatedForUser";
        public const string UserNotYetOnboarded = "UserNotYetOnboarded";
        public const string StartingUserRequest = "StartingUserRequest";
        public const string ExceptionWhileWelcomingUser = "ExceptionWhileWelcomingUser";
        public const string UserRequestHasBeenProccessed = "UserRequestHasBeenProccessed";
        public const string ExceptionAfterProcessingUserRequest = "ExceptionAfterProcessingUserRequest";

        //Bot.Onboarding
        public const string StartingOnboardingProcess = "StartingOnboardingProcess";
        public const string AskingForVehicleInformation = "AskingForVehicleInformation";
        public const string VehicleInformationAlreadyOnBoarded = "VehicleInformationAlreadyOnBoarded";
        public const string UserWantsToAddVehicleInformation = "UserWantsToAddVehicleInformation";
        public const string UserDoesNotWantsToAddVehicleInformation = "UserDoesNotWantsToAddVehicleInformation";
        public const string AddingVehicleInformation = "AddingVehicleInformation";
        public const string ExceptionWhileAddingVehicleInformation = "ExceptionWhileAddingVehicleInformation";
        public const string AskingHowManyCommutersCanSitInVehicle = "AskingHowManyCommutersCanSitInVehicle";
        public const string SettingNumberOfCommutersThatCanSitInVehicle = "SettingNumberOfCommutersThatCanSitInVehicle";
        public const string UserResponseInvalid = "UserResponseInvalid";
        public const string ExceptionSettingNumberOfCommutersThatCanSitInVehicle = "ExceptionSettingNumberOfCommutersThatCanSitInVehicle";
        public const string OfficeAlreadySetAddHome = "OfficeAlreadySetAddHome";
        public const string AddingOfficeOfUser = "AddingOfficeOfUser";
        public const string OfficeAddedAskForHomeLocation = "OfficeAddedAskForHomeLocation";
        public const string ExceptionWhileSettingOfficeAddress = "ExceptionWhileSettingOfficeAddress";
        public const string HomeLocationAlreadyOnboarded = "HomeLocationAlreadyOnboarded";
        public const string UnableToGeFacebookMessage = "UnableToGeFacebookMessage";
        public const string GettingCommuterFromFacebookId = "GettingCommuterFromFacebookId";
        public const string SettingHomeLocationOfCommuter = "SettingHomeLocationOfCommuter";
        public const string OnboardingIsComplete = "OnboardingIsComplete";
        public const string ExceptionWhileSettingHomeAddress = "ExceptionWhileSettingHomeAddress";

        //Bot.External
        public const string AuthenticatingUser = "AuthenticatingUser";

        //Bot.CommuterManager
        public const string AddingCommuterToState = "AddingCommuterToState";
        public const string CommuterNotAddedToState = "CommuterNotAddedToState";
        public const string AddOfficeOfCommuterToStateAndDb = "AddOfficeOfCommuterToStateAndDb";
        public const string OfficeAddedToDb = "OfficeAddedToDb";
        public const string OfficeAddedToState = "OfficeAddedToState";
        public const string ErrorWhileAddingOfficeLocationToDbRevertingChanges = "ErrorWhileAddingOfficeLocationToDbRevertingChanges";
        public const string AddHomeOfCommuterToStateAndDb = "AddHomeOfCommuterToStateAndDb";
        public const string HomeAddedToDb = "HomeAddedToDb";
        public const string HomeAddedToState = "HomeAddedToState";
        public const string ErrorWhileAddingHomeLocationToDbRevertingChanges = "ErrorWhileAddingHomeLocationToDbRevertingChanges";

        //Bot.Data.EfModels
        public const string AddingCommutersToEntityFrameworkSqlDb = "AddingCommuterToEntityFrameworkSqlDb";
        public const string CommutersAddedToEntityFrameworkSqlDb = "CommutersAddedToEntityFrameworkSqlDb";
        public const string ExceptionWhileAddingCommutersToEntityFrameworkSqlDb = "ExceptionWhileAddingCommutersToEntityFrameworkSqlDb";
        public const string CommuterAddedToState = "CommuterAddedToState";
        public const string ExceptionWhileStartingOnboarding = "ExceptionWhileStartingOnboarding";
        public const string UpdatingCommutersToEntityFrameworkSqlDb = "UpdatingCommutersToEntityFrameworkSqlDb";
        public const string OneOrMoreCommuterDoesNotExist = "OneOrMoreCommuterDoesNotExist";
        public const string CommutersUpdatedToEntityFrameworkSqlDb = "CommutersUpdatedToEntityFrameworkSqlDb";
        public const string ExceptionWhileUpdatingCommutersToEntityFrameworkSqlDb = "ExceptionWhileUpdatingCommutersToEntityFrameworkSqlDb";
        public const string AddingTripRequestToEntityFrameworkSqlDb = "AddingTripRequestToEntityFrameworkSqlDb";
        public const string AddedTripRequestToEntityFrameworkSqlDb = "AddedTripRequestToEntityFrameworkSqlDb";
        public const string ExceptionWhileAddingTripToEntityFrameworkSqlDb = "ExceptionWhileAddingTripToEntityFrameworkSqlDb";

        //Bot.Request
        public const string UserResponseForWhereDoYouWantToGo = "UserResponseForWhereDoYouWantToGo";
        public const string UserIsAlreadyGoingSomewhere = "UserIsAlreadyGoingSomewhere";
        public const string UserWantsToGoToOffice = "UserWantsToGoToOffice";
        public const string UserWantsToGoHome = "UserWantsToGoHome";
        public const string AskingForTransportMode = "AskingForTransportMode";
        public const string UserHasAlreadyDecidedOnGoingHow = "UserHasAlreadyDecidedOnGoingHow";
        public const string UserDecidedToDrive = "UserDecidedToDrive";
        public const string UserDecidedToJoin = "UserDecidedToJoin";
        public const string ExceptionWhileDecidingTransportMode = "ExceptionWhileDecidingTransportMode";
        public const string CreatingTripRequestForUser = "CreatingTripRequestForUser";
        public const string SendingTripRequestToRequestManager = "SendingTripRequestToRequestManager";
        public const string TripRequestCreated = "TripRequestCreated";
        public const string TripRequestCreationFailed = "TripRequestCreationFailed";

        //Bot.Data.TripRequestManager
        public const string AddTripRequestStarted = "AddTripRequestStarted";
        public const string AddingTripRequestToState = "AddingTripRequestToState";
        public const string AddedRequestToState = "AddedRequestToState";
        public const string FailedToAddTripRequestToState = "FailedToAddTripRequestToState";
        public const string PublishingOwnerRequestMessage = "PublishingOwnerRequestMessage";

        //Bot.Worker ProcessTripOwnerRequest
        public const string HandleProcessTripOwnerRequestMessageBegin = "HandleProcessTripOwnerRequestMessageBegin";
        public const string HandleProcessTripOwnerRequestMessageEnd = "HandleProcessTripOwnerRequestMessageEnd";
        public const string HandleProcessTripOwnerRequestMessageFailed = "HandleProcessTripOwnerRequestMessageFailed";
        public const string HandleProcessTripOwnerRequestMessageException = "HandleProcessTripOwnerRequestMessageException";

        //Bot.Worker AddPoolersToTripMessage
        public const string HandleAddPoolersToTripMessageBegin = "HandleAddPoolersToTripMessageBegin";
        public const string HandleAddPoolersToTripMessageEnd = "HandleAddPoolersToTripMessageEnd";
        public const string HandleAddPoolersToTripMessageFailed = "HandleAddPoolersToTripMessageFailed";
        public const string HandleAddPoolersToTripMessageException = "HandleAddPoolersToTripMessageException";

        //Bot.Worker EndTripMessage
        public const string HandleEndTripMessageBegin = "HandleEndTripMessageBegin";
        public const string HandleEndTripMessageEnd = "HandleEndTripMessageEnd";
        public const string HandleEndTripMessageFailed = "HandleEndTripMessageFailed";
        public const string HandleEndTripMessageException = "HandleEndTripMessageException";

        //Bot.Worker GetPoolersInTripMessage
        public const string HandleGetPoolersInTripMessageBegin = "HandleGetPoolersInTripMessageBegin";
        public const string HandleGetPoolersInTripMessageEnd = "HandleGetPoolersInTripMessageEnd";
        public const string HandleGetPoolersInTripMessageFailed = "HandleGetPoolersInTripMessageFailed";
        public const string HandleGetPoolersInTripMessageException = "HandleGetPoolersInTripMessageException";

        //Bot.Worker GetTripRouteMessage
        public const string HandleGetTripRouteMessageBegin = "HandleGetTripRouteMessageBegin";
        public const string HandleGetTripRouteMessageEnd = "HandleGetTripRouteMessageEnd";
        public const string HandleGetTripRouteMessageFailed = "HandleGetTripRouteMessageFailed";
        public const string HandleGetTripRouteMessageException = "HandleGetTripRouteMessageException";

        //Bot.External Maps
        public const string GetRouteToOffice = "GetRouteToOffice";
        public const string GetRouteToHome = "GetRouteToHome";
        public const string GetRouteBetweenCoordinates = "GetRouteBetweenCoordinates";
        public const string FaileToGetRouteBetweenPoints = "FaileToGetRouteBetweenPoints";
        public const string GotRouteBetweenPoints = "GotRouteBetweenPoints";
        public const string GotRouteBetweenPointsFromGoogle = "GotRouteBetweenPointsFromGoogle";
        public const string UnableToAddCommuterToState = "UnableToAddCommuterToState";

        //Bot.Worker.Core 
        public const string UnableToGetRouteForTheTrip = "UnableToGetRouteForTheTrip";
        public const string UnableToAddCommuterRequestToState = "UnableToAddCommuterRequestToState";
        public const string GotRouteForTheRequest = "GotRouteForTheRequest";
        public const string UnableToUpdateRequestStatus = "UnableToUpdateRequestStatus";
        public const string AddedCommuterToState = "AddedCommuterToState";
        public const string AddingPoolersToState = "AddingPoolersToState";
        public const string UnableToAddPoolersToState = "UnableToAddPoolersToState";
        public const string ProcessModelDoesNotExists = "ProcessModelDoesNotExists";
        public const string GotPoolersToRequest = "GotPoolersToRequest";
        public const string UpdatingRequestStatusOfPoolerRequest = "UpdatingRequestStatusOfPoolerRequest";
        public const string UpdatedRequestStatusOfPoolerRequest = "UpdatedRequestStatusOfPoolerRequest";
        public const string UpdatRequestStatusOfPoolerRequestFailed = "UpdatRequestStatusOfPoolerRequestFailed";
        public const string ExceptionWhileaddingRequestToState = "ExceptionWhileaddingRequestToState";
        public const string FailedToAddPoolers = "FailedToAddPoolers";
        public const string AcceptedPoolersRemovedFromProcessModel = "AcceptedPoolersRemovedFromProcessModel";
        public const string RejectedPoolersSetToInitialized = "RejectedPoolersSetToInitialized";
        public const string AddingMorePoolersToState = "AddingMorePoolersToState";
        public const string MarkingRequestStateInTrip = "MarkingRequestStateInTrip";
        public const string CompletingCommuterRequest = "CompletingCommuterRequest";
        public const string RemovingOwnerRequestFromState = "RemovingOwnerRequestFromState";
        public const string RemovingPoolerRequestsFromState = "RemovingPoolerRequestsFromState";
        public const string GotProcessModel = "GotProcessModel";
        public const string GotProcessModelForTripRoute = "GotProcessModelForTripRoute";

        //Bot.Models TripRequestManger
        public const string UpdatingRequestStatus = "UpdatingRequestStatus";
        public const string RequestGoingHowIsInvalid = "RequestGoingHowIsInvalid";
        public const string RequestGoingToIsInvalid = "RequestGoingToIsInvalid";
        public const string RequestNodeDoesNotExistsInState = "RequestDoesNotExistsInState";
        public const string RequestDoesNotExistsInState = "RequestDoesNotExistsInState";
        public const string RequestDidNotUpdateInStore = "RequestDidNotUpdateInStore";
        public const string RequestStatusUpdated = "RequestStatusUpdated";
        public const string UpdateRequestStatusFailed = "UpdateRequestStatusFailed";
        public const string UpdatedRequestStatus = "UpdatedRequestStatus";
        public const string AddingPoolersToTripAndUpdatePoolerStatus = "AddingPoolersToTripAndUpdatePoolerStatus";
        public const string UpdatingRequestStatusFailed = "UpdatingRequestStatusFailed";

        public const string VehicleOwnerAddedtoStateBegin = "VehicleOwnerAddedtoStateBegin";
        public const string VehicleOwnerIsNotInState = "VehicleOwnerIsNotInState";
        public const string BotChannelConfigNotFound = "BotChannelConfigNotFound";
    }
}
