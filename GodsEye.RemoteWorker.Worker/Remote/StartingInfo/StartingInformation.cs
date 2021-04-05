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

    public class StartingInformation
    {
        public SiwInformation Siw { get; set; }
    }
}
