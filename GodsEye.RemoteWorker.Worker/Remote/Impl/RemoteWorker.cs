using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Worker.FacialAnalysis;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;
using GodsEye.Utility.Application.Items.Constants.String;
using GodsEye.Utility.Application.Items.Messages.MasterToSlave.Impl.Requests;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public partial class RemoteWorker : IRemoteWorker
    {
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

            //register all the handlers
            await RegisterHandlersAsync(cameraIp, cameraPort);

            //start the siw worker
            await _streamingImageWorker
                .StartAsync(cameraPort, cameraIp, _parentCancellationTokenSource);
        }
    }
}
