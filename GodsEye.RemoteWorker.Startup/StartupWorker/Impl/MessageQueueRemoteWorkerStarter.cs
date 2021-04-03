using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private readonly ISet<Task> _failedTasks = new HashSet<Task>();
        private readonly ConcurrentBag<Task> _activeWorkerTasks = new ConcurrentBag<Task>();


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
                //wait until each worker finishes
                foreach (var activeWorkerTask in _activeWorkerTasks)
                {
                    //do not consider the failed processes
                    if (_failedTasks.Contains(activeWorkerTask))
                    {
                        continue;
                    }

                    //wait for process to complete 
                    try
                    {
                        await activeWorkerTask;
                    }
                    catch (Exception)
                    {
                        _failedTasks.Add(activeWorkerTask);
                    }
                }

                //not busy wait
                await Task.Delay(1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void OnMessageFromCamera(OnlineCameraMessage message)
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

            //attempt to create the worker
            _activeWorkerTasks.Add(
                siwService.StartAsync(cameraPort, cameraIp));
        }

    }
}
