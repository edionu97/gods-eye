using System;
using EasyNetQ;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Streaming;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.Utility.Application.Items.Constants.Message;
using GodsEye.Utility.Application.Items.Messages.Registration;

namespace GodsEye.RemoteWorker.Startup.StartupWorker.Impl
{
    public class MessageQueueRemoteWorkerStarter : IMessageQueueRemoteWorkerStarter
    {
        private readonly IBus _messageBus;
        private readonly IServiceProvider _serviceProvider;

        public MessageQueueRemoteWorkerStarter(
            IBus messageQueue, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _messageBus = messageQueue;
        }

        public async Task StartAsync()
        {
            //register the handler
           _messageBus?.PubSub
                .SubscribeAsync<OnlineCameraMessage>(
                    StringConstants.CameraToBussQueueName, OnMessageFromCamera);

            //non busy wait
            while (true)
            {
                await Task.Delay(1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task OnMessageFromCamera(OnlineCameraMessage message)
        {
            //deconstruct the message
            var (cameraIp, cameraPort) = message;

            //get the siw service
            var siwService = _serviceProvider
                .GetService<IStreamingImageWorker>();

            //ignore the 
            if (siwService == null)
            {
                return;
            }

            await siwService.StartAsync(0);
        }
    }
}
