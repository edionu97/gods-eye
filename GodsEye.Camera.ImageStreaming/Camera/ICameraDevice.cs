using System.Threading.Tasks;

namespace GodsEye.Camera.ImageStreaming.Camera
{
    public interface ICameraDevice
    {
        public Task StartSendingImageFrames(string deviceId, int devicePort);
    }
}
