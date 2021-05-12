using System;
using EasyNetQ;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Worker.FacialAnalysis;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;
using GodsEye.RemoteWorker.Workers.Messages;
using GodsEye.RemoteWorker.Workers.Messages.Requests;
using GodsEye.RemoteWorker.Workers.Messages.Responses;
using GodsEye.Utility.Application.Items.Geolocation.Model;
using Microsoft.Extensions.Logging;

using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public partial class RemoteWorker
    {
        private readonly ConcurrentDictionary<string, (Task<SearchForPersonResponse>, CancellationTokenSource)> _currentActiveWorkersForSearching =
            new ConcurrentDictionary<string, (Task<SearchForPersonResponse>, CancellationTokenSource)>();

        private readonly ConcurrentDictionary<string, IRequestResponseMessage> _cancelRequests
            = new ConcurrentDictionary<string, IRequestResponseMessage>();

        private void HandleTheSearchForPersonRequest(
            SearchForPersonMessageRequest messageRequest,
            SiwInformation information, 
            GeolocationInfo cameraGeolocation,
            CancellationToken parentToken)
        {
            //get the service
            var facialAnalysisWorkerInstance =
                _serviceProvider
                    .GetService<IFacialAnalysisAndRecognitionWorker>()
                ?? throw new ArgumentNullException();

            //create a linked parentToken source, relative to the parent
            //if child gets cancelled => parent is alive
            //if parent gets cancelled => all children are cancelled 
            var cancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(parentToken);

            //destruct the camera ip and port
            var (cameraIp, cameraPort) = information;

            //create a new recognition task
            var recognitionTask = facialAnalysisWorkerInstance
                .StartSearchingForPersonAsync(
                    new FarwStartingInformation
                    {
                        FrameBuffer = _streamingImageWorker.FrameBuffer,
                        StatisticsInformation = (cameraIp, cameraPort),
                        SearchedPersonBase64Img = messageRequest.MessageContent,
                        OnBufferProcessed = (response, startTime, endTime) =>
                        {
                            //unpack the object
                            var (detectionInfo, frameInfo) = response;

                            //create a new job and pass it to the job executor
                            _jobExecutor.QueueJob(async () =>
                            {
                                //get the analysis response
                                FacialAttributeAnalysisResponse analysisResponse = null;
                                try
                                {
                                    //get the response of the facial analysis
                                    analysisResponse = await facialAnalysisWorkerInstance
                                        .AnalyzeFaceAndExtractFacialAttributesAsync(
                                            frameInfo,
                                            detectionInfo.FaceRecognitionInfo.First().FaceBoundingBox,
                                            cancellationTokenSource.Token);
                                }
                                catch (Exception e)
                                {
                                    _logger?.LogCritical(e.Message);
                                }

                                //person potentially found
                                _messageBus.PubSub
                                    // ReSharper disable once MethodSupportsCancellation
                                    // ReSharper disable once MethodHasAsyncOverload
                                    .Publish(new PersonFoundMessageResponse
                                    {
                                        IsFound = true,
                                        MessageContent = (detectionInfo, frameInfo, analysisResponse),
                                        MessageId = messageRequest.MessageId,
                                        StartTimeUtc = startTime,
                                        EndTimeUtc = endTime,
                                        FromLocation = cameraGeolocation
                                    });

                            });
                        }
                    },
                    cancellationTokenSource.Token);

            //if there is already a worker that handles the identification for a specific person do nothing
            if (_currentActiveWorkersForSearching.ContainsKey(messageRequest.MessageId))
            {
                return;
            }

            //register the worker as online
            _currentActiveWorkersForSearching
                .TryAdd(messageRequest.MessageId, (recognitionTask, cancellationTokenSource));

            //handle the case in which the request is not canceled before start
            if (!_cancelRequests.ContainsKey(messageRequest.MessageId))
            {
                return;
            }

            //log the information
            _logger.LogInformation(Constants.BlacklistedRequest);

            //get stop the client
            HandleTheStopSearchingForPersonMessage(_cancelRequests[messageRequest.MessageId]);
        }

        /// <summary>
        /// Stops the search for a specific node
        /// </summary>
        /// <param name="message">the message </param>
        private void HandleTheStopSearchingForPersonMessage(IRequestResponseMessage message)
        {
            //get the message id
            var id = message.MessageId;

            //handle the empty id case
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            //add this into the dictionary
            _cancelRequests.TryAdd(id, message);

            //if there is no worker that handles that request return nothing
            if (!_currentActiveWorkersForSearching.TryGetValue(id, out var recognitionDetails))
            {
                //log the message
                _logger.LogInformation(Constants.PostponedTheRequestMessage);
                return;
            }

            //unpack the object
            var (_, cancellationTokenSource) = recognitionDetails;

            //cancel the recognition worker
            try
            {
                //do the cancellation
                cancellationTokenSource.Cancel();

                //remove from the dictionary
                _cancelRequests.TryRemove(id, out _);
                _currentActiveWorkersForSearching.TryRemove(id, out _);
            }
            catch (Exception e)
            {
                //log the exception
                _logger.LogCritical(e.Message);
            }
        }

        private void HandleTheActiveWorkersMessage(
            IRequestResponseMessage requestMessage, GeolocationInfo geolocation)
        {
            //get the id of the workers
            var workersJobs = new List<string>();

            //iterate the active workers
            foreach (var (hashId, (task, _)) in _currentActiveWorkersForSearching)
            {
                //skip if the task is not running
                if (task.IsFaulted || task.IsCanceled || task.IsCompleted || task.IsCompletedSuccessfully)
                {
                    continue;
                }

                //add the hash id in the list
                workersJobs.Add(hashId);
            }

            //person potentially found
            _messageBus.PubSub
                // ReSharper disable once MethodSupportsCancellation
                // ReSharper disable once MethodHasAsyncOverload
                .Publish(new ActiveWorkerMessageResponse
                {
                    MessageId = requestMessage.MessageId,
                    Geolocation = geolocation,
                    MessageContent = (_workerIdentificationNumber, workersJobs)
                });
        }
    }
}
