using System.Collections.Concurrent;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Remote.Messages;

namespace GodsEye.RemoteWorker.Worker.Remote.StartingInfo
{
    public class SiwInformation
    {
        public string CameraIp { get; set; }
        public int CameraPort { get; set; }

        public void Deconstruct(
            out string cameraIp, out int cameraPort)
        {
            cameraIp = CameraIp;
            cameraPort = CameraPort;
        }
    }

    public class RwStartingInformation
    {
        public SiwInformation Siw { get; set; }

        public ConcurrentBag<IMessage> NotProcessedRequests { get; set; }
    }
}
