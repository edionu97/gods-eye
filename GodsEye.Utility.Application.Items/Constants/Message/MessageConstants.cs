namespace GodsEye.Utility.Application.Items.Constants.Message
{
    public class MessageConstants
    {
        public static class PathHelpers
        {
            public static string PathNullOrEmptyMessage => "The entered path '{0}' is not valid";
        }

        public static class CameraDevice
        {
            public static string CameraIsInitializedMessage => "Camera is up and running";

            public static string CameraIsStreamingMessage => "Camera is ready for streaming images on port '{0}'...";

            public static string CameraIsStreamingImagesMessage => "Streaming location found, started streaming images. Camera details: {0}";

            public static string StreamingLocationLostMessage => "The communication with streaming location lost.";
        }

        public static class Workers
        {
            public static string WebsocketServerCouldNotBeStartMessage => "Web socket server could not be started";

            public static string WebSocketListeningOnPortMessage => "Started web socket server on {0}:{1}";

            public static string TryingToConnectOnCameraMessage => "Trying to connect to camera: {0}:{1}";

            public static string ConnectionStatusSuccessfulMessage => "Connection  status: successful";

            public static string ConnectionStatusFailedMessage => "Connection  status: failed";
        }

    }
}
