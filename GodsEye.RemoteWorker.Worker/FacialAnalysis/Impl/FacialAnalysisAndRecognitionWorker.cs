using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodsEye.DataStreaming.LoadShedding.Manager;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;
using GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer;
using GodsEye.Utility.Application.Helpers.Helpers.Threading;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
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
                await WaitHelpers.WaitWhileAsync(() => !frameBuffer.IsReady, cancellationToken);
                await WaitHelpers.WaitWhileAsync(() => frameBuffer.InputRate <= 0, cancellationToken);

                //start the searching
                do
                {
                    //compute the response
                    var response = await
                        ProcessTheFrameBufferAsync(
                            frameBuffer,
                            (message, token) =>
                                _facialAnalysisService.SearchPersonInImageAsync(personBase64Img,
                                    message.ImageBase64EncodedBytes, token),
                            (r) => r.FaceRecognitionInfo.Any(),
                            cancellationToken, (logger, cameraIp, cameraPort));


                } while (!cancellationToken.IsCancellationRequested);

            }, cancellationToken);
        }

        /// <summary>
        /// This method it is used for processing the frame buffer
        /// </summary>
        /// <typeparam name="T">type of the response</typeparam>
        /// <param name="frameBuffer">the instance of the frame buffer</param>
        /// <param name="bufferProcessor">the function that processes the buffer</param>
        /// <param name="stoppingPredicate">the stopping condition</param>
        /// <param name="token">the cancellation token</param>
        /// <param name="loggingInfo">the logging information</param>
        /// <returns>a task containing the result or null if nothing could not be found</returns>
        public async Task<T> ProcessTheFrameBufferAsync<T>(
            IFrameBuffer frameBuffer,
            Func<NetworkImageFrameMessage, CancellationToken, Task<T>> bufferProcessor,
            Predicate<T> stoppingPredicate,
            CancellationToken token, (ILogger, string, int) loggingInfo) where T : class
        {
            //deconstruct the logging information
            var (logger, cameraIp, cameraPort) = loggingInfo;

            //get the working snapshot
            var snapShot = frameBuffer.TakeASnapshot();

            logger.LogDebug($"Snapshot-ed the frame buffer, the number of total frames read is: {snapShot.Count}");

            //begin a logging scope
            using (logger.BeginScope($"Recognition Job for camera {cameraIp}:{cameraPort}"))
            {
                //create a new watch
                var watch = Stopwatch.StartNew();

                //start the searching
                var totalProcessingTime = .0;
                while (snapShot.Any() && !token.IsCancellationRequested)
                {
                    //unpack the data tuple value
                    var (_, message) = snapShot.Dequeue();

                    //do the buffer processing and count the times
                    var beforeElapsed = watch.Elapsed;
                    var response = await bufferProcessor(message, token);
                    var frameProcessingTime = (watch.Elapsed - beforeElapsed).TotalSeconds;

                    //continue with next frames if the person is not found
                    if (stoppingPredicate?.Invoke(response) == true)
                    {
                        return response;
                    }

                    //increment the total processing time
                    totalProcessingTime += frameProcessingTime;

                    //load shed the data if needed
                    snapShot = await _policyManager
                        .ApplyLoadSheddingPolicyAsync(
                            snapShot,
                            frameBuffer.InputRate - totalProcessingTime,
                            frameProcessingTime);
                }
            }

            //null response
            return null;
        }
    }
}
