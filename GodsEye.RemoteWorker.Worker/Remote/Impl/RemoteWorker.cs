using System;
using EasyNetQ;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Workers.Messages;
using GodsEye.RemoteWorker.Workers.Messages.Requests;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;

using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public partial class RemoteWorker : IRemoteWorker
    {
        private ILogger _logger;
        private readonly IBus _messageBus;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStreamingImageWorker _streamingImageWorker;
        private readonly CancellationTokenSource _parentCancellationTokenSource;

        public RemoteWorker(
            IBus bus,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            IStreamingImageWorker streamingImageWorker)
        {
            _parentCancellationTokenSource = new CancellationTokenSource();

            _messageBus = bus;
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;
            _streamingImageWorker = streamingImageWorker;
        }

        public async Task ConfigureWorkersAndStartAsync(RwStartingInformation rwStartingInformation)
        {
            //get the starting information
            var (cameraIp, cameraPort) = rwStartingInformation.Siw;

            //create the logger
            _logger ??= _loggerFactory.CreateLogger(string.
                Format(Constants.RemoteWorkerLoggerName, cameraIp, cameraPort));

            //handle all the requests that are not yet processed
            foreach (var notProcessedRequest in rwStartingInformation.NotProcessedRequests)
            {
                await CheckForNewRequestAsync(notProcessedRequest, rwStartingInformation);
            }

            //start the siw worker
            await _streamingImageWorker
                .StartAsync(cameraPort, cameraIp, _parentCancellationTokenSource);
        }

        public Task CheckForNewRequestAsync(IRequestResponseMessage requestMessage, RwStartingInformation rwStartingInformation)
        {
            //based on the request message type do the right action
            switch (requestMessage)
            {
                //handle the search for person response message
                case SearchForPersonMessage searchForPersonMessage:
                    {
                        //if there is already a thread that handles the searching request do nothing
                        if (_currentActiveWorkersForSearching.ContainsKey(searchForPersonMessage.MessageId))
                        {
                            break;
                        }

                        //processing request
                        _logger.LogInformation(
                            Constants.ProcessingRequestMessage, nameof(SearchForPersonMessage));

                        //handle the search person request
                        HandleTheSearchForPersonRequest(
                            searchForPersonMessage,
                            rwStartingInformation.Siw,
                            _parentCancellationTokenSource.Token);
                        break;
                    }

                //handle the stop searching for person message
                case StopSearchingForPersonMessage stopSearchingForPersonMessage:
                    {
                        //processing request
                        _logger.LogInformation(
                            Constants.ProcessingRequestMessage, nameof(StopSearchingForPersonMessage));

                        //handle the stop searching for person
                        HandleTheStopSearchingForPersonMessage(stopSearchingForPersonMessage);
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
