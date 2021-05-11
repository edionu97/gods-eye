using System;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer;

namespace GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo
{
    public class FarwStartingInformation
    {
        public IFrameBuffer FrameBuffer { get; set; }

        public string SearchedPersonBase64Img { get; set; }

        public (string UsingCameraIp, int UsingCameraPort) StatisticsInformation { get; set; }

        /// <summary>
        /// This is a callback that is executed on the caller's thread
        /// </summary>
        public Action<(SearchForPersonResponse, string), DateTime, DateTime> OnBufferProcessed { get; set; }

        public void Deconstruct(
            out IFrameBuffer frameBuffer, 
            out string searchedPersonBase64Img, 
            out (string UsingCameraIp, int UsingCameraPort) statisticsInformation,
            out Action<(SearchForPersonResponse, string), DateTime, DateTime> onBufferProcessed)
        {
            frameBuffer = FrameBuffer;
            onBufferProcessed = OnBufferProcessed;
            searchedPersonBase64Img = SearchedPersonBase64Img;
            statisticsInformation = StatisticsInformation;
        }
    }
}
