using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;
using GodsEye.Utility.Application.Helpers.Helpers.Threading;
using Microsoft.Extensions.Logging;

namespace GodsEye.RemoteWorker.Worker.FacialAnalysis.Impl
{
    public class FacialAnalysisAndRecognitionWorker : IFacialAnalysisAndRecognitionWorker
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFacialRecognitionAndAnalysisService _facialAnalysisService;

        public FarwStartingInformation AnalysisSummary { get; private set; }

        public FacialAnalysisAndRecognitionWorker(
            ILoggerFactory loggerFactory,
            IFacialRecognitionAndAnalysisService facialAnalysisService)
        {
            _loggerFactory = loggerFactory;
            _facialAnalysisService = facialAnalysisService;
        }


        public Task StartSearchingForPersonAsync(FarwStartingInformation startingInformation, CancellationTokenSource cancellationToken)
        {
            //set the analysis summary 
            AnalysisSummary = startingInformation;

            //deconstruct the object
            var (frameBuffer,
                 personBase64Img,
                 (cameraIp, cameraPort)) = AnalysisSummary;


            //run in another thread the recognition job
            return Task.Run(async () =>
            {
                //
                var logger =
                    _loggerFactory.CreateLogger($"FacialAnalysisAndRecognition started for {cameraIp}:{cameraPort}");

                logger.LogInformation("Wait until the frame buffer is full");

                await WaitHelpers.WaitWhile(() => !frameBuffer.IsReady);

                logger.LogInformation("SS done");
                //for (var isFound = false; !cancellationToken.IsCancellationRequested && !isFound;)
                //{


                //    logger.LogInformation("Snapshot values {0}", frameBuffer.TakeASnapshot().Count);

                //}
            });
        }
    }
}
