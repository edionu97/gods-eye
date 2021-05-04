using System;
using EasyNetQ;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GodsEye.RemoteWorker.Worker.Remote.Messages.Requests;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;
using IMessage = GodsEye.RemoteWorker.Worker.Remote.Messages.IMessage;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public partial class RemoteWorker : IRemoteWorker
    {
        private readonly IBus _messageBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStreamingImageWorker _streamingImageWorker;
        private readonly CancellationTokenSource _parentCancellationTokenSource;
        private readonly IList<ISubscriptionResult> _subscriptionResults = new List<ISubscriptionResult>();

        public RemoteWorker(
            IBus bus,
            IServiceProvider serviceProvider,
            IStreamingImageWorker streamingImageWorker)
        {
            _parentCancellationTokenSource = new CancellationTokenSource();

            _messageBus = bus;
            _serviceProvider = serviceProvider;
            _streamingImageWorker = streamingImageWorker;
        }

        public async Task ConfigureWorkersAndStartAsync(RwStartingInformation rwStartingInformation)
        {
            //get the starting information
            var (cameraIp, cameraPort) = rwStartingInformation.Siw;

            //handle all the requests that are not yet processed
            foreach (var notProcessedRequest in rwStartingInformation.NotProcessedRequests)
            {
                await CheckForNewRequestAsync(notProcessedRequest, rwStartingInformation);
            }


            //register all the handlers
            //await RegisterHandlersAsync(cameraIp, cameraPort);

            //start the siw worker
            await _streamingImageWorker
                .StartAsync(cameraPort, cameraIp, _parentCancellationTokenSource);
        }

        public Task CheckForNewRequestAsync(IMessage requestMessage, RwStartingInformation rwStartingInformation)
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

                        HandleTheSearchForPersonRequest(
                            searchForPersonMessage,
                            rwStartingInformation.Siw,
                            _parentCancellationTokenSource.Token);
                        break;
                    }

                //handle the stop searching for person message
                case StopSearchingForPersonMessage stopSearchingForPersonMessage:
                    {
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


        private async Task RemoveUnusedQueuesAsync()
        {
            //delete the used queues
            foreach (var subscriptionResult in _subscriptionResults)
            {
                //dispose all the queues
                try
                {
                    await _messageBus.Advanced.QueueDeleteAsync(subscriptionResult.Queue);
                }
                catch (Exception)
                {
                    //queue disposed
                }
            }
        }
    }
}
