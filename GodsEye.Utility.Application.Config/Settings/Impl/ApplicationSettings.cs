using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Config.Configuration;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;

namespace GodsEye.Utility.Application.Config.Settings.Impl
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string CameraId { get; private set; }
        public int StreamingWidth { get; private set; }
        public int StreamingHeight { get; private set; }
        public int FramesPerSecond { get; private set; }
        public string CameraAddress { get; private set; }
        public int CameraStreamingPort { get; private set; }
        public string ImageDirectoryPath { get; private set; }
        public ImageType StreamingImageType { get; private set; }

        public ApplicationSettings(IAppConfig configuration)
        {
            SetCameraConfig(configuration.Camera);
        }

        private void SetCameraConfig(CameraSectionConfig configuration)
        {
            CameraId = configuration?.CameraId;
            CameraAddress = configuration?.Network?.CamerasLocation;
            ImageDirectoryPath = configuration?.ImageDirectoryPath;
            CameraStreamingPort = (configuration?.Network?.StreamsOnPort).GetValueOrDefault();
            FramesPerSecond = (configuration?.ImageOptions?.FramesPerSecond).GetValueOrDefault();
            StreamingImageType = (configuration?.Network?.ImageStreamingFormat).GetValueOrDefault();
            StreamingWidth = (configuration?.ImageOptions?.ImageResolution?.Width).GetValueOrDefault();
            StreamingHeight = (configuration?.ImageOptions?.ImageResolution?.Height).GetValueOrDefault();
        }
    }
}
