namespace GodsEye.Utility.Application.Items.Constants.Message
{
    public class MessageConstants
    {
        public static class Services
        {
            public static string UserNotFoundMessage => "Wrong username or password";

            public static string UserFoundMessage => "User already exists";
        }

        public static class Repository
        {
            public static string NotFoundMessage => "The object could not be found";
        }

        public static class PathHelpers
        {
            public static string PathNullOrEmptyMessage => "The entered path '{0}' is not valid";
        }

        public static class ContentHasherHelpers
        {
            public static string StringNullOrEmptyMessage => "The string must not be null or empty";
        }

        public static class Ws
        {
            public static string AutoGeneratedClientMessage => "Autogenerated client enabled";

            public static string ClientWasAutoGeneratedSuccessfullyMessage =>
                "Client for this ws server was autogenerated, path to client: {0}\n";

            public static string ClientCouldNotBeGeneratedMessage =>
                "The client  for ws({0}:{1}) could not be generated\n";

            public static string MessageFromClientMessage => "Received message from client, the message is {0}";
        }

        public static class FrameBuffer
        {
            public static string FrameBufferAutoAdjustedMessage =>
                "The frame buffer size auto adjusted(f={0}, t={1}) in order to keep up with the input rate({2})\n";
        }

        public static class CameraDevice
        {
            public static string CameraIsInitializedMessage => "Camera is up and running\n";

            public static string CameraIsStreamingMessage => "Camera is ready for streaming images on port '{0}'...\n";

            public static string CameraIsStreamingImagesMessage => "Streaming location found, started streaming images. Camera details: {0}\n";

            public static string StreamingLocationLostMessage => "The communication with streaming location lost.\n";

            public static string GettingCameraGeolocationMessage => "Computing camera geographical location...\n";

            public static string CameraLocationSuccessfullyDeterminedMessage => "Camera geolocation successfully determined\n";

            public static string LocationCouldNotBeDeterminedMessage => "Location could not be determined, due to {0}\n";
        }

        public static class Workers
        {
            public static string WebsocketServerCouldNotBeStartMessage => "Web socket server could not be started\n";

            public static string WebSocketListeningOnPortMessage => "Started web socket server on {0}:{1}\n";

            public static string TryingToConnectOnCameraMessage => "Trying to connect to camera: {0}:{1}\n";

            public static string ConnectionStatusSuccessfulMessage => "Connection  status: successful\n";

            public static string ConnectionStatusFailedMessage =>
                "There was a problem with communication with camera: {0}:{1}. Stopped worker execution\n";

            public static string ReceivedMessageFromQueueMessage => "Received '{0}' from queue";

            public static string WaitingToReceiveMessageFromClientsMessage => "Receiving online messages started, listening message from queue: '{0}'\n";

            public static string WorkerHasBeenTerminatedMessage => "The worker has been terminated, worker id: {0}\n";

            public static string ProblemStartingWorkerMessage => "There was a problem starting the worker ({0}:{1})\n";

            public static string FarwStartedLoggerScopeMessage => "FacialAnalysisAndRecognition for {0}:{1}";

            public static string TheWorkerWillStopMessage => "This error will make the worker to stop...\n";

            public static string FarwStartedStoppedMessage => "The facial recognition worker has stopped\n";

            public static string FarwWaitUntilTheBufferIsFullMessage => "Waiting until the buffer is full\n";

            public static string FarwWorkerStartedMessage => "Worker started\n";

            public static string FarwSnapshotedBufferMessage =>
                "Snapshot-ed the frame buffer, the number of total frames read is: {0}\n";

            public static string FarwRoundFinishedMessage => "Buffer processed in {0} sec\n";

            public static string FarwJobDetailsMessage => "AI job for camera {0}:{1}";

            public static string FarwFacialAnalysisStarted => "Facial analysis started";

            public static string BroadcastingRequestMessage => "Broadcasting the request to all the available workers";

            public static string RemoteWorkerLoggerName =>
                "Remote logger is online, starting connection with camera('{0}':'{1}')";

            public static string ProcessingRequestMessage => "Processing request '{0}'\n";

            public static string PostponedTheRequestMessage =>
                "There does not exist yet a worker for that request, postponed the cancellation...\n";

            public static string BlacklistedRequest =>
                "Identified that the searching request was cancelled, stopping its processing...\n";

            public static string ExtraTimeMessage => "The extra-time that will be added to next round is: {0}s\n";
        }

        public static class LoadShedding
        {
            public static string LoadSheddingShouldBePerformedMessage => "Load shedding of the data is required";

            public static string NoLoadSheddingRequiredMessage => "No load shedding of the data is required";

            public static string TheAvailableTimeForDataProcessingMessage => "Time available for processing the frame buffer is: {0} sec";

            public static string WrongGenerationParametersMessage => "The maximum number of values that can be generated are {0}";

            public static string TupleRequiresXSecondsToBeProcessedMessage => "One tuple requires {0} sec to be processed";

            public static string HeuristicLoadSheddingScopeMessage => "Heuristic load shedding for reducing initial data size f={0} t={1}";

            public static string HeuristicLoadSheddingFirstRoundFinishedMessage => "After first round the number of items kept are: {0}. Applying the next round: {1}\n";

            public static string HeuristicLoadSheddingDataSizeMessage => "Load shed-ed data size={0}\n";
        }
    }
}
