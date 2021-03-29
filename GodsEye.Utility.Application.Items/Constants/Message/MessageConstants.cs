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

    }
}
