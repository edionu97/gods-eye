using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Worker.FacialAnalysis;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public class RemoteWorker : IRemoteWorker
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly IStreamingImageWorker _streamingImageWorker;
        private readonly IFacialAnalysisAndRecognitionWorker _facialAnalysisAndRecognitionWorker;

        public RemoteWorker(
            ILoggerFactory loggerFactory,
            IStreamingImageWorker streamingImageWorker,
            IFacialAnalysisAndRecognitionWorker facialAnalysisAndRecognition)
        {
            _cancellationToken = new CancellationTokenSource();

            _loggerFactory = loggerFactory;
            _streamingImageWorker = streamingImageWorker;
            _facialAnalysisAndRecognitionWorker = facialAnalysisAndRecognition;
        }

        public async Task ConfigureWorkersAndStartAsync(RwStartingInformation rwStartingInformation)
        {
            //get the starting information
            var (cameraIp, cameraPort) = rwStartingInformation.Siw;

            //start the farw worker
            var _ = _facialAnalysisAndRecognitionWorker
                .StartSearchingForPersonAsync(
                    new FarwStartingInformation
                    {
                        FrameBuffer = _streamingImageWorker.FrameBuffer,
                        StatisticsInformation = (cameraIp, cameraPort),
                        SearchedPersonBase64Img = await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\rob.txt")
                    },
                    _cancellationToken);

            //start the siw worker
            await _streamingImageWorker
                .StartAsync(cameraPort, cameraIp, _cancellationToken);
        }
    }
}
