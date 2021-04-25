using GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer;

namespace GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo
{
    public class FarwStartingInformation
    {
        public IFrameBuffer FrameBuffer { get; set; }

        public string SearchedPersonBase64Img { get; set; }

        public (string UsingCameraIp, int UsingCameraPort) StatisticsInformation { get; set; }

        public void Deconstruct(
            out IFrameBuffer frameBuffer, 
            out string searchedPersonBase64Img, 
            out (string UsingCameraIp, int UsingCameraPort) statisticsInformation)
        {
            frameBuffer = FrameBuffer;
            searchedPersonBase64Img = SearchedPersonBase64Img;
            statisticsInformation = StatisticsInformation;
        }
    }
}
