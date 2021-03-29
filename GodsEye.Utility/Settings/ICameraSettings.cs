using GodsEye.Utility.Enums;

namespace GodsEye.Utility.Settings
{
    public interface ICameraSettings
    {
        public string CameraId { get; }
        public int StreamingWidth { get;}
        public int StreamingHeight { get; }
        public int FramesPerSecond { get; }
        public string CameraAddress { get; }
        public int CameraStreamingPort { get;}
        public ImageType StreamingImageType { get; }
        public string ImageDirectoryPath { get; }
    }
}
