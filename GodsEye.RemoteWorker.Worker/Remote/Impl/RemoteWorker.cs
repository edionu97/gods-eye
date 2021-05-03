using System;
using EasyNetQ;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;

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

            try
            {
                //register all the handlers
                await RegisterHandlersAsync(cameraIp, cameraPort);

                //start the siw worker
                await _streamingImageWorker
                    .StartAsync(cameraPort, cameraIp, _parentCancellationTokenSource);
            }
            finally
            {
                //remove the unused queues async
                await RemoveUnusedQueuesAsync();
            }
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
