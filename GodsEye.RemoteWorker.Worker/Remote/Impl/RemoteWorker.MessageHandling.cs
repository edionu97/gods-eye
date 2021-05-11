using System;
using EasyNetQ;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Worker.FacialAnalysis;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;
using GodsEye.RemoteWorker.Worker.Remote.Messages;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;
using GodsEye.RemoteWorker.Workers.Messages;
using GodsEye.RemoteWorker.Workers.Messages.Requests;
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
            SearchForPersonMessage message,
            SiwInformation information, CancellationToken parentToken)
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
                        SearchedPersonBase64Img = message.MessageContent,
                        OnBufferProcessed = (response, startTime, endTime) =>
                        {
                            //person potentially found
                            _messageBus.PubSub
                                 // ReSharper disable once MethodSupportsCancellation
                                 .Publish(new PersonFoundMessage
                                 {
                                     IsFound = true,
                                     MessageContent = response,
                                     MessageId = message.MessageId,
                                     StartTimeUtc = startTime,
                                     EndTimeUtc = endTime
                                 });
                        }
                    },
                    cancellationTokenSource.Token);

            //if there is already a worker that handles the identification for a specific person do nothing
            if (_currentActiveWorkersForSearching.ContainsKey(message.MessageId))
            {
                return;
            }

            //register the worker as online
            _currentActiveWorkersForSearching
                .TryAdd(message.MessageId, (recognitionTask, cancellationTokenSource));

            //handle the case in which the request is not canceled before start
            if (!_cancelRequests.ContainsKey(message.MessageId))
            {
                return;
            }

            //log the information
            _logger.LogInformation(Constants.BlacklistedRequest);

            //get stop the client
            HandleTheStopSearchingForPersonMessage(_cancelRequests[message.MessageId]);
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
    }
}
