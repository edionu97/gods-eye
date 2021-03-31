using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Config.Configuration;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;

namespace GodsEye.Utility.Application.Config.Settings.Impl
{
    public class ApplicationSettings : IApplicationSettings
    {
        #region Camera
        public string CameraId { get; private set; }
        public int StreamingWidth { get; private set; }
        public int StreamingHeight { get; private set; }
        public int FramesPerSecond { get; private set; }
        public string CameraAddress { get; private set; }
        public int CameraStreamingPort { get; private set; }
        public string ImageDirectoryPath { get; private set; }
        public ImageType StreamingImageType { get; private set; }
        #endregion

        #region RemoteWorker
        public int ServerStartingPort { get; set; }
        public string ServerAddress { get; set; }
        #endregion

        public ApplicationSettings(IAppConfig configuration)
        {
            //set the camera config
            SetCameraConfig(configuration.Camera);

            //set the remote worker config
            SetRemoteWorkerConfig(configuration.RemoteWorker);
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

        private void SetRemoteWorkerConfig(RemoteWorkerSectionConfig configuration)
        {
            ServerAddress = configuration?.WebSocket?.ServerAddress;
            ServerStartingPort = (configuration?.WebSocket?.ServerStartingPort).GetValueOrDefault();
        }

    }
}
