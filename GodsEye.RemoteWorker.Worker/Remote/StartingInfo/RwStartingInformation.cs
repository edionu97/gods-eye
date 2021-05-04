using System.Collections.Concurrent;
using GodsEye.RemoteWorker.Workers.Messages;

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

        public ConcurrentBag<IRequestResponseMessage> NotProcessedRequests { get; set; }
    }
}
