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
using GodsEye.RemoteWorker.Workers.Messages.Components;
using GodsEye.RemoteWorker.Workers.Messages.Requests;
using GodsEye.RemoteWorker.Workers.Messages.Responses;
using GodsEye.Utility.Application.Items.Geolocation.Model;
using Grpc.Core;
using Microsoft.Extensions.Logging;

using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public partial class RemoteWorker
    {
        private readonly ConcurrentDictionary<string, (Task<SearchForPersonResponse>, CancellationTokenSource, JobSummary)> _currentActiveWorkersForSearching =
            new ConcurrentDictionary<string, (Task<SearchForPersonResponse>, CancellationTokenSource, JobSummary)>();

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

            //save the date when the search was initialized
            var initializationTime = DateTime.UtcNow;

            //create a new recognition task
            var recognitionTask = facialAnalysisWorkerInstance
                .StartSearchingForPersonAsync(
                    new FarwStartingInformation
                    {
                        FrameBuffer = _streamingImageWorker.FrameBuffer,
                        StatisticsInformation = (cameraIp, cameraPort),
                        SearchedPersonBase64Img = messageRequest.MessageContent,
                        OnBufferProcessed = (response, _, endTime) =>
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
                                        FindByWorkerId = _workerIdentificationNumber,
                                        IsFound = true,
                                        MessageContent = (detectionInfo, frameInfo, analysisResponse),
                                        MessageId = messageRequest.MessageId,
                                        UserId = messageRequest.UserId,
                                        StartTimeUtc = initializationTime,
                                        EndTimeUtc = endTime,
                                        FromLocation = cameraGeolocation,
                                        SearchedPersonImageBase64 = messageRequest.MessageContent
                                    });

                            });
                        },
                        OnFailure = (failureCause) =>
                        {
                            //create the failure message
                            var failureMessage = new ActiveWorkerFailedMessageResponse
                            {
                                UserId = messageRequest.UserId,
                                MessageId = messageRequest.MessageId,
                                FailureSummary = new FailureSummary
                                {
                                    ExceptionType = failureCause?.GetType().Name,
                                    FailureDetails = failureCause?.Message,
                                    Status = failureCause?.Message
                                }
                            };

                            //if the failure cause id rpc exception
                            //extract the exact messages from the exception
                            if (failureCause is RpcException rpcException)
                            {
                                //get the exception status
                                var status = rpcException.Status;

                                //set the status
                                failureMessage.FailureSummary.Status = status.StatusCode.ToString();

                                //set the details
                                failureMessage.FailureSummary.FailureDetails = status.Detail;
                            }
                                 
                            //send the failure message to the invoker of this request
                            _messageBus.PubSub
                                // ReSharper disable once MethodSupportsCancellation
                                // ReSharper disable once MethodHasAsyncOverload
                                .Publish(failureMessage);
                        }
                    },
                    cancellationTokenSource.Token);

            //if there is already a worker that handles the identification for a specific person do nothing
            if (_currentActiveWorkersForSearching.ContainsKey(messageRequest.MessageId))
            {
                return;
            }

            //create the job summary
            var jobSummary = new JobSummary
            {
                JobHashId = messageRequest.MessageId,
                SearchedImage = messageRequest.MessageContent,
                SubmittedOn = initializationTime,
                CreatedByUserId = messageRequest.UserId
            };

            //register the worker as online
            _currentActiveWorkersForSearching
                .TryAdd(messageRequest.MessageId, (recognitionTask, cancellationTokenSource, jobSummary));

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
            var (_, cancellationTokenSource, _) = recognitionDetails;

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
            var jobSummaries = new List<JobSummary>();

            //iterate the active workers
            foreach (var (_, (task, _, jobSummary)) in _currentActiveWorkersForSearching)
            {
                //skip if the task is not running
                if (task.IsFaulted || task.IsCanceled || task.IsCompleted || task.IsCompletedSuccessfully)
                {
                    continue;
                }

                //skip if the job is not created by the current user
                if (jobSummary.CreatedByUserId != requestMessage.UserId)
                {
                    continue; ;
                }

                //add the hash id in the list
                jobSummaries.Add(jobSummary);
            }

            //person potentially found
            _messageBus.PubSub
                // ReSharper disable once MethodSupportsCancellation
                // ReSharper disable once MethodHasAsyncOverload
                .Publish(new ActiveWorkerMessageResponse
                {
                    MessageId = requestMessage.MessageId,
                    StartedAt = _startedOnAt,
                    UserId = requestMessage.UserId,
                    Geolocation = geolocation,
                    MessageContent = (_workerIdentificationNumber, jobSummaries)
                });
        }
    }
}
