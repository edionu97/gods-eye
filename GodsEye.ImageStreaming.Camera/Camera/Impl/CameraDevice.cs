using System;
using System.Threading.Tasks;
using GodsEye.Utility.Configuration;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;

namespace GodsEye.ImageStreaming.Camera.Camera.Impl
{
    public class CameraDevice : ICameraDevice
    {
        private readonly IImageProvider _imageProvider;
        private readonly IApplicationSettings _applicationSettings;

        public CameraDevice(IImageProvider imageProvider, IApplicationSettings applicationSettings)
        {
            _imageProvider = imageProvider;
            _applicationSettings = applicationSettings;
        }

        public Task StartTackingShots(int deviceId)
        {

            throw new NotImplementedException();
        }
    }
}
