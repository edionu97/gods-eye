using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.DataStreaming.LoadShedding.Manager;
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
        private readonly ILoadSheddingFixedPolicyManager _policyManager;
        private readonly IFacialRecognitionAndAnalysisService _facialAnalysisService;

        public FarwStartingInformation AnalysisSummary { get; private set; }

        public FacialAnalysisAndRecognitionWorker(
            ILoggerFactory loggerFactory,
            IFacialRecognitionAndAnalysisService facialAnalysisService,
            ILoadSheddingFixedPolicyManager loadSheddingManager)
        {
            _loggerFactory = loggerFactory;
            _policyManager = loadSheddingManager;
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

                //start the searching
                double? lastRoundAvgProcessingTime = null;
                do
                {
                    //compute the response
                    var (response, roundAvgTime) =
                        await DoASearchingRoundAsync(
                            frameBuffer,
                            personBase64Img,
                            lastRoundAvgProcessingTime,
                            cancellationToken);

                    logger.LogInformation($"Processing rate: {roundAvgTime / 1000} seconds per frame");
                    logger.LogInformation($"Input rate: {frameBuffer.InputRate} frames");

                    //set the last avg processing time to current round processing time
                    lastRoundAvgProcessingTime = roundAvgTime;

                } while (!cancellationToken.IsCancellationRequested);

            }, cancellationToken);
        }

        public async Task<(SearchForPersonResponse, double)> DoASearchingRoundAsync(
            IFrameBuffer frameBuffer,
            string personBase64Img,
            double? lastRoundAvgProcessingTime,
            CancellationToken token)
        {
            //compute the processing rate and the input rate
            var (processingRateFps, inputRateFps) =
                ConvertInputAndProcessingRateToFps(frameBuffer, lastRoundAvgProcessingTime);

            //get the working snapshot
            //do the load shedding if necessary
            var snapShot = await _policyManager
                .SyncUsedFixedPolicyAsync(
                    frameBuffer.TakeASnapshot(), processingRateFps, inputRateFps);

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

                //call the server method
                var response = await _facialAnalysisService
                    .SearchPersonInImageAsync(personBase64Img, message.ImageBase64EncodedBytes, token);

                //compute the call time
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

        /// <summary>
        /// This function it is used for converting the processing times into fps rates
        /// </summary>
        /// <param name="frameBuffer">the frame buffer that contains info about the input rate</param>
        /// <param name="lastRoundProcessingTime">the avg time for last round</param>
        /// <returns>a pair of items (the processingRate as Fps, input rate as Fps)</returns>
        private static (double, double) ConvertInputAndProcessingRateToFps(IFrameBuffer frameBuffer, double? lastRoundProcessingTime)
        {
            //get the tuple input rate in fps
            var tupleInputRateFps = frameBuffer.InputRate;

            //if the value is null we need to adjust it such as the value of the avgProcessingRateFps == avgProcessingRateFps
            //so in that case it should be 1000
            var avgProcessingRateTime = lastRoundProcessingTime ?? 1000;

            //transform the fps in time (total sending time)
            var tupleInputRateAsTime = tupleInputRateFps * 1000.0;

            //transform the processing rate in frames per second
            var avgProcessingRateFps = tupleInputRateAsTime / avgProcessingRateTime;

            //return the processing rate and the input rate in frames per second
            return (avgProcessingRateFps, tupleInputRateFps);
        }
    }
}
