using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;
using GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer;
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


        public Task StartSearchingForPersonAsync(FarwStartingInformation startingInformation, CancellationToken cancellationToken)
        {
            //set the analysis summary 
            AnalysisSummary = startingInformation;

            //deconstruct the object
            var (frameBuffer,
                 personBase64Img,
                 (cameraIp, cameraPort)) = AnalysisSummary;

            //
            var logger =
                _loggerFactory.CreateLogger($"FacialAnalysisAndRecognition started for {cameraIp}:{cameraPort}");

            //run in another thread the recognition job
            return Task.Run(async () =>
            {
                //wait until the frame buffer is full and the input rate is calculated
                await WaitHelpers.WaitWhile(() => !frameBuffer.IsReady);
                await WaitHelpers.WaitWhile(() => frameBuffer.InputRate <= 0);

                var avgProcessingTime = .0;
                do
                {
                    //compute the response
                    var (response, roundAvgTime) =
                        await DoASearchingRoundAsync(frameBuffer, personBase64Img, cancellationToken);

                    logger.LogInformation($"Processing rate: {roundAvgTime/1000} seconds per frame");
                    logger.LogInformation($"Input rate: {frameBuffer.InputRate} frames");

                    avgProcessingTime = roundAvgTime;

                } while (!cancellationToken.IsCancellationRequested);

            }, cancellationToken);
        }

        public async Task<(SearchForPersonResponse, double)> DoASearchingRoundAsync(IFrameBuffer frameBuffer, string personBase64Img, CancellationToken token)
        {
            //take the snapshot
            var snapShot = frameBuffer.TakeASnapshot();

            //remember the number of total frames
            var totalFramesToProcess = snapShot.Count;

            //count the number of values from buffer
            var averageFrameProcessingTime = .0;

            //create a new watch
            var watch = Stopwatch.StartNew();

            //start the searching
            while (snapShot.Any() && !token.IsCancellationRequested)
            {
                //unpack the data tuple value
                var (_, message) = snapShot.Dequeue();

                //get the response and it's processing time
                var currentFrameProcessingTime = watch.Elapsed;
                var response = await _facialAnalysisService
                    .SearchPersonInImageAsync(personBase64Img, message.ImageBase64EncodedBytes, token);
                averageFrameProcessingTime += (watch.Elapsed - currentFrameProcessingTime).TotalMilliseconds;

                //continue with next frames if the person is not found
                if (!response.FaceRecognitionInfo.Any())
                {
                    continue;
                }

                //return response
                return (response, Math.Ceiling(averageFrameProcessingTime / totalFramesToProcess));
            }

            //null response
            return (null, Math.Ceiling(averageFrameProcessingTime / totalFramesToProcess));
        }
    }
}
