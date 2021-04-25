using System;
using EasyNetQ;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using GodsEye.RemoteWorker.Worker.Remote;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;
using GodsEye.RemoteWorker.Worker.Streaming;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Constants;
using GodsEye.Utility.Application.Items.Constants.Message;
using GodsEye.Utility.Application.Items.Constants.String;
using GodsEye.Utility.Application.Items.Messages.Registration;

using LocalConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Startup.StartupWorker.Impl
{
    public class MessageQueueRemoteWorkerStarter : IMessageQueueRemoteWorkerStarter
    {
        private readonly IBus _messageBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageQueueRemoteWorkerStarter> _logger;

        private readonly ISet<Task> _failedTasks = new HashSet<Task>();
        private readonly ConcurrentBag<Task> _activeWorkerTasks = new ConcurrentBag<Task>();


        public MessageQueueRemoteWorkerStarter(
            IBus messageQueue,
            IServiceProvider serviceProvider,
            ILogger<MessageQueueRemoteWorkerStarter> logger)
        {
            _logger = logger;
            _messageBus = messageQueue;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync()
        {
            //register the handler
            _messageBus?.PubSub
                 .SubscribeAsync<OnlineCameraMessage>(
                     StringConstants.CameraToBussQueueName, OnMessageFromCamera);

            //log the message
            _logger
                .LogInformation(LocalConstants
                    .WaitingToReceiveMessageFromClientsMessage, StringConstants.CameraToBussQueueName);

            //do the waiting
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
                    catch (Exception e)
                    {
                        _failedTasks.Add(activeWorkerTask);

                        //log the message
                        _logger
                            .LogWarning(LocalConstants.WorkerHasBeenTerminatedMessage, e.Message);
                    }
                }

                //not busy wait
                await Task.Delay(1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void OnMessageFromCamera(OnlineCameraMessage message)
        {
            //log the message in the same scope
            using (_logger.BeginScope(
                LocalConstants.ReceivedMessageFromQueueMessage, nameof(OnlineCameraMessage)))
            {
                //log the received message
                _logger
                    .LogInformation(
                        $"{JsonSerializerDeserializer<OnlineCameraMessage>.Serialize(message)}\n");
            }

            //deconstruct the message
            var (cameraIp, cameraPort) = message;
            try
            {
                //get the siw service
                var remoteWorker = _serviceProvider
                    .GetService<IRemoteWorker>();

                //ignore the 
                if (remoteWorker == null)
                {
                    return;
                }

                //attempt to create the worker
                _activeWorkerTasks.Add(remoteWorker
                    .ConfigureWorkersAndStartAsync(new RwStartingInformation
                    {
                        Siw = new SiwInformation
                        {
                            CameraIp = cameraIp,
                            CameraPort = cameraPort
                        }
                    }));
            }
            catch (Exception e)
            {
                //log the failure message
                using (_logger.BeginScope($"{e.GetType()}: {e.Message}"))
                {
                    _logger.LogCritical(LocalConstants
                        .ProblemStartingWorkerMessage, cameraIp, cameraPort);
                }

                //throw the exception back
                throw;
            }
        }

    }
}
