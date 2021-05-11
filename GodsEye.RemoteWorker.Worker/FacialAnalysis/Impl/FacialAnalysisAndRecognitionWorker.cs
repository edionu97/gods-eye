using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.DataStreaming.LoadShedding.Manager;
using GodsEye.Utility.Application.Config.BaseConfig;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy;
using GodsEye.Utility.Application.Helpers.Helpers.Threading;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Args;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
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

        public Task<FacialAttributeAnalysisResponse>
            AnalyzeFaceAndExtractFacialAttributesAsync(
                string base64Image, 
                FaceLocationBoundingBox faceLocation, CancellationToken token)
        {
            return _facialAnalysisService.AnalyseFaceAsync(base64Image, faceLocation, token);
        }

        public Task<SearchForPersonResponse>
            StartSearchingForPersonAsync(FarwStartingInformation startingInformation, CancellationToken cancellationToken)
        {
            //set the analysis summary 
            AnalysisSummary = startingInformation;

            //deconstruct the object
            var (frameBuffer,
                 personBase64Img,
                 (cameraIp, cameraPort), _) = AnalysisSummary;

            //create the logger fot the 
            var logger = _loggerFactory
                .CreateLogger(string
                    .Format(Constants.FarwStartedLoggerScopeMessage, cameraIp, cameraPort));

            //run in another thread the recognition job
            return Task.Run(async () =>
            {
                //wait for buffer to become full
                await WaitHelpers.WaitWhileAsync(() => frameBuffer.InputRate <= 0, cancellationToken);

                //wait until the buffer is full
                if (_config.StartWorkerOnlyWhenBufferIsFull)
                {
                    //log the message
                    logger.LogInformation(Constants.FarwWaitUntilTheBufferIsFullMessage);

                    //wait until the frame buffer is full and the input rate is calculated
                    await WaitHelpers.WaitWhileAsync(() => !frameBuffer.IsReady, cancellationToken);
                }

                //start the worker
                try
                {
                    //log the starting message
                    logger.LogInformation(Constants.FarwWorkerStartedMessage);

                    //set the extra args for the ls policy
                    _policyManager.PolicyArgs = new LoadSheddingPolicyArgs
                    {
                        SearchedImageBase64 = personBase64Img
                    };

                    //start the searching rounds
                    do
                    {
                        //compute the response
                        var (response, _) = await ProcessTheFrameBufferAsync(logger, cancellationToken);

                        //stop the cycle if we have the response and we are configured to do so
                        if (response != null && _config.StopWorkerOnFirstPositiveAnswer)
                        {
                            return response;
                        }

                        //repeat until the request is fulfilled or canceled
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

                //return null 
                return null;

            }, cancellationToken);
        }

        /// <summary>
        /// This method it is used for processing the frame buffer
        /// </summary>
        /// <param name="logger">the logger used for logging the messages</param>
        /// <param name="cancellationToken">the cancellation token</param>
        /// <returns>a task containing the result or null if nothing could not be found</returns>
        private async Task<(SearchForPersonResponse, string)> 
            ProcessTheFrameBufferAsync(ILogger logger, CancellationToken cancellationToken)
        {
            //get the starting time of the round
            var startTime = DateTime.UtcNow;

            //destruct the object
            var (frameBuffer,
                searchedPersonImage,
                (cameraIp, cameraPort),
                onBufferProcessed) = AnalysisSummary;

            //get the working snapshot
            var snapShot = frameBuffer.TakeASnapshot();

            //create the last positive response
            (SearchForPersonResponse, string) lastPositiveResponse = (null, null);

            //begin a logging scope
            using (logger.BeginScope(string.Format(Constants.FarwJobDetailsMessage, cameraIp, cameraPort)))
            {
                //log the message
                logger.LogInformation(string
                    .Format(Constants.FarwSnapshotedBufferMessage, snapShot.Count));

                //create a new watch
                var watch = Stopwatch.StartNew();

                //start the searching
                var totalProcessingTime = .0;
                while (snapShot.Any() && !cancellationToken.IsCancellationRequested)
                {
                    //unpack the data tuple value
                    var (_, message) = snapShot.Dequeue();

                    //count the initial time
                    var beforeElapsed = watch.Elapsed;

                    //do the server call
                    var response = await _facialAnalysisService
                        .SearchPersonInImageAsync(
                            searchedPersonImage,
                            message.ImageBase64EncodedBytes,
                            cancellationToken);

                    //count the elapsed time in seconds
                    var frameProcessingTime = (watch.Elapsed - beforeElapsed).TotalSeconds;

                    //if the there is a match
                    if (response.FaceRecognitionInfo.Any())
                    {
                        //compute the last positive response
                        lastPositiveResponse = (response, message.ImageBase64EncodedBytes);

                        //invoke the callback on positive response
                        onBufferProcessed?.Invoke(lastPositiveResponse, startTime, DateTime.UtcNow);
                    }

                    //stop the buffer processing if we discover an result 
                    //only if this is enabled from config
                    if (response.FaceRecognitionInfo.Any() && _config.StopWorkerOnFirstPositiveAnswer)
                    {
                        return lastPositiveResponse;
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

                //log the message
                logger.LogInformation(string
                    .Format(Constants.FarwRoundFinishedMessage, totalProcessingTime));
            }

            //null response
            return lastPositiveResponse;
        }
    }
}
