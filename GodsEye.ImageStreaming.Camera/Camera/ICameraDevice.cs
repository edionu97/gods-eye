using System.Threading.Tasks;

namespace GodsEye.ImageStreaming.Camera.Camera
{
    public interface ICameraDevice
    {
        public Task StartTackingShots(string deviceId, int devicePort);
    }
}
