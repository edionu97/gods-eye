namespace GodsEye.Utility.Constants.Message
{
    public class MessageConstants
    {
        public static class PathHelpers
        {
            public static string PathNullOrEmptyMessage => "The entered path '{0}' is not valid";
        }

        public static class CameraDevice
        {
            public static string CameraIsStreamingMessage => "Camera '{0}' is ready for streaming images on port '{1}'...";

            public static string CameraIsStreamingImagesMessage => "Streaming location found, started streaming images.\n Camera details: {0}";
        }

    }
}
