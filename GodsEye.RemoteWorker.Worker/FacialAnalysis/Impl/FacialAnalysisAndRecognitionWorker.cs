using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.DataStreaming.LoadShedding.Manager;
using GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy;
using GodsEye.Utility.Application.Helpers.Helpers.Threading;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Worker.FacialAnalysis.Impl
{
    public class FacialAnalysisAndRecognitionWorker : IFacialAnalysisAndRecognitionWorker
    {
        private readonly FacialAnalysisAndRecognitionWorkerConfig _config;

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILoadSheddingFixedPolicyManager _policyManager;
        private readonly IFacialRecognitionAndAnalysisService _facialAnalysisService;

        public FarwStartingInformation AnalysisSummary { get; private set; }

        public FacialAnalysisAndRecognitionWorker(
            IConfig config,
            ILoggerFactory loggerFactory,
            IFacialRecognitionAndAnalysisService facialAnalysisService,
            ILoadSheddingFixedPolicyManager loadSheddingManager)
        {
            _loggerFactory = loggerFactory;
            _policyManager = loadSheddingManager;
            _facialAnalysisService = facialAnalysisService;
            _config = config.Get<FacialAnalysisAndRecognitionWorkerConfig>();
        }

        public Task StartSearchingForPersonAsync(FarwStartingInformation startingInformation, CancellationToken cancellationToken)
        {
            //set the analysis summary 
            AnalysisSummary = startingInformation;

            //deconstruct the object
            var (frameBuffer,
                 personBase64Img,
                 (cameraIp, cameraPort)) = AnalysisSummary;

            //create the logger fot the 
            var logger =_loggerFactory
                .CreateLogger(string
                    .Format(Constants.FarwStartedLoggerScopeMessage, cameraIp, cameraPort));

            //run in another thread the recognition job
            return Task.Run(async () =>
            {
                //wait until the frame buffer is full and the input rate is calculated
                await WaitHelpers.WaitWhileAsync(() => !frameBuffer.IsReady, cancellationToken);

                //wait until the buffer is full
                if (_config.StartWorkerOnlyWhenBufferIsFull)
                {
                    //log the message
                    logger.LogInformation(Constants.FarwWaitUntilTheBufferIsFullMessage);

                    //wait for buffer to become full
                    await WaitHelpers.WaitWhileAsync(() => frameBuffer.InputRate <= 0, cancellationToken);
                }

                //start the worker
                try
                {
                    //log the starting message
                    logger.LogDebug(Constants.FarwWorkerStartedMessage);

                    //start the searching
                    do
                    {
                        //compute the response
                        var response = await
                            ProcessTheFrameBufferAsync(
                                frameBuffer,
                                (message, token) =>
                                    _facialAnalysisService.SearchPersonInImageAsync(
                                        personBase64Img,
                                        message.ImageBase64EncodedBytes,
                                        token),
                                (r) => r.FaceRecognitionInfo.Any(),
                                cancellationToken, (logger, cameraIp, cameraPort));


                    } while (!cancellationToken.IsCancellationRequested);
                }
                catch (Exception e)
                {
                    //log the failure message
                    using (logger.BeginScope($"{e.GetType()}: {e.Message}"))
                    {
                        logger.LogCritical(Constants.TheWorkerWillStopMessage);
                    }

                    throw;
                }
                finally
                {
                    logger.LogInformation(Constants.FarwStartedStoppedMessage);
                }

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

            //log the message
            logger.LogDebug(string
                .Format(Constants.FarwSnapshotedBufferMessage, snapShot.Count));

            //begin a logging scope
            using (logger.BeginScope(string.Format(Constants.FarwRecognitionJobMessage, cameraIp, cameraPort)))
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
