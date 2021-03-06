using System;
using EasyNetQ;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Workers.Messages;
using GodsEye.RemoteWorker.Workers.Messages.Requests;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;
using GodsEye.Utility.Application.Items.Executors;
using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public partial class RemoteWorker : IRemoteWorker
    {
        private readonly Guid _workerIdentificationNumber;
        private readonly CancellationTokenSource _parentCancellationTokenSource;

        private ILogger _logger;
        private DateTime _startedOnAt;
        private readonly IBus _messageBus;
        private readonly IJobExecutor _jobExecutor;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStreamingImageWorker _streamingImageWorker;

        public RemoteWorker(
            IBus bus,
            IJobExecutor jobExecutor,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            IStreamingImageWorker streamingImageWorker)
        {
            _workerIdentificationNumber = Guid.NewGuid();
            _parentCancellationTokenSource = new CancellationTokenSource();

            _messageBus = bus;
            _jobExecutor = jobExecutor;
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;
            _streamingImageWorker = streamingImageWorker;
        }

        public async Task ConfigureWorkersAndStartAsync(RwStartingInformation rwStartingInformation)
        {
            //get the starting information
            var (cameraIp, cameraPort) = rwStartingInformation.Siw;

            //set the starting on date
            _startedOnAt = DateTime.UtcNow;

            //create the logger
            _logger ??= _loggerFactory.CreateLogger(string.
                Format(Constants.RemoteWorkerLoggerName, cameraIp, cameraPort));

            //handle all the requests that are not yet processed
            foreach (var notProcessedRequest in rwStartingInformation.NotProcessedRequests)
            {
                await CheckForNewRequestAsync(notProcessedRequest, rwStartingInformation);
            }

            try
            {

                //start the siw worker
                await _streamingImageWorker
                    .StartAsync(cameraPort, cameraIp, _parentCancellationTokenSource);
            }
            finally
            {
                //stop the job executor for current instance 
                try
                {
                    _jobExecutor.StopJobExecution();
                }
                catch (Exception e)
                {
                    _logger?.LogCritical(e.Message);
                }
            }
        }

        public Task CheckForNewRequestAsync(IRequestResponseMessage requestMessage, RwStartingInformation rwStartingInformation)
        {
            //based on the request message type do the right action
            switch (requestMessage)
            {
                //handle the search for person response message
                case SearchForPersonMessageRequest searchForPersonMessage:
                    {
                        //if there is already a thread that handles the searching request do nothing
                        if (_currentActiveWorkersForSearching.ContainsKey(searchForPersonMessage.MessageId))
                        {
                            //get the active task that handles the search for person request
                            var (task, _, _) = _currentActiveWorkersForSearching[searchForPersonMessage.MessageId];

                            //if the task is still running 
                            if (!(task.IsFaulted || task.IsCanceled || task.IsCompleted || task.IsCompletedSuccessfully))
                            {
                                break;
                            }

                            //remove the precedent task because is no longer active
                            _currentActiveWorkersForSearching.TryRemove(searchForPersonMessage.MessageId, out _);
                        }

                        //processing request
                        _logger.LogInformation(
                            Constants.ProcessingRequestMessage, nameof(SearchForPersonMessageRequest));

                        //handle the search person request
                        HandleTheSearchForPersonRequest(
                            searchForPersonMessage,
                            rwStartingInformation.Siw,
                            rwStartingInformation.CameraGeolocation,
                            _parentCancellationTokenSource.Token);
                        break;
                    }

                //handle the stop searching for person message
                case StopSearchingForPersonMessageRequest stopSearchingForPersonMessage:
                    {
                        //processing request
                        _logger.LogInformation(
                            Constants.ProcessingRequestMessage, nameof(StopSearchingForPersonMessageRequest));

                        //handle the stop searching for person
                        HandleTheStopSearchingForPersonMessage(stopSearchingForPersonMessage);
                        break;
                    }

                //handle the active workers message
                case GetActiveWorkersMessageRequest activeWorkersMessage:
                    {
                        //processing request
                        _logger.LogInformation(
                            Constants.ProcessingRequestMessage, nameof(GetActiveWorkersMessageRequest));

                        //handle the active worker requests
                        HandleTheActiveWorkersMessage(activeWorkersMessage, rwStartingInformation.CameraGeolocation);
                        break;
                    }

                //throw exception if none of above
                default:
                    {
                        throw new Exception();
                    }
            }

            //return a completed task
            return Task.CompletedTask;
        }
    }
}
