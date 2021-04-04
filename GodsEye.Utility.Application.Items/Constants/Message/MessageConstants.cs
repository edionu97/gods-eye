﻿namespace GodsEye.Utility.Application.Items.Constants.Message
{
    public class MessageConstants
    {
        public static class PathHelpers
        {
            public static string PathNullOrEmptyMessage => "The entered path '{0}' is not valid";
        }

        public static class CameraDevice
        {
            public static string CameraIsInitializedMessage => "Camera is up and running\n";

            public static string CameraIsStreamingMessage => "Camera is ready for streaming images on port '{0}'...\n";

            public static string CameraIsStreamingImagesMessage => "Streaming location found, started streaming images. Camera details: {0}\n";

            public static string StreamingLocationLostMessage => "The communication with streaming location lost.\n";
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

        }
    }
}
