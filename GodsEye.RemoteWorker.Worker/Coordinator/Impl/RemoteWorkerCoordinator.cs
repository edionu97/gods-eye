using System;
using EasyNetQ;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using GodsEye.RemoteWorker.Worker.Remote;
using GodsEye.RemoteWorker.Workers.Messages;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;
using GodsEye.Utility.Application.Items.Constants.String;
using GodsEye.Utility.Application.Items.Messages.Registration;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;

using LocalConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Worker.Coordinator.Impl
{
    public class RemoteWorkerCoordinator : IRemoteWorkerCoordinatorStarter
    {
        private readonly IBus _messageBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RemoteWorkerCoordinator> _logger;

        private readonly ISet<Task> _failedTasks = new HashSet<Task>();

        private readonly ConcurrentBag<IRequestResponseMessage> _activeRequests
            = new ConcurrentBag<IRequestResponseMessage>();

        private readonly ConcurrentBag<(Task, IRemoteWorker, RwStartingInformation)> _activeWorkerTasks =
            new ConcurrentBag<(Task, IRemoteWorker, RwStartingInformation)>();

        public RemoteWorkerCoordinator(
            IBus messageQueue,
            IServiceProvider serviceProvider,
            ILogger<RemoteWorkerCoordinator> logger)
        {
            _logger = logger;
            _messageBus = messageQueue;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync()
        {
            //register the handler
            await _messageBus.PubSub
                  .SubscribeAsync<OnlineCameraMessage>(
                      StringConstants.CameraToBussQueueName, OnMessageFromCamera);

            //register the handler for search for person message
            await _messageBus.PubSub.SubscribeAsync<IRequestResponseMessage>(
                StringConstants.MasterToSlaveBusQueueName,
                async r =>
                {
                    //add the request in bag for new workers
                    _activeRequests.Add(r);

                    //log the message and the information
                    _logger.LogInformation(JsonSerializerDeserializer<dynamic>.Serialize(new
                    {
                        RequestReceived = r?.GetType().Name,
                        Message = LocalConstants.BroadcastingRequestMessage
                    }) +'\n');

                    //distribute the work among the active workers
                    foreach (var (activeWorkerTask, worker, startingInformation) in _activeWorkerTasks)
                    {
                        //ignore the finished tasks
                        if (activeWorkerTask.IsCanceled
                            || activeWorkerTask.IsFaulted
                            || activeWorkerTask.IsCompleted
                            || activeWorkerTask.IsCompletedSuccessfully)
                        {
                            continue;
                        }

                        //distribute the work
                        await worker.CheckForNewRequestAsync(r, startingInformation);
                    }
                });

            //log the message
            _logger
                .LogInformation(LocalConstants
                    .WaitingToReceiveMessageFromClientsMessage, StringConstants.CameraToBussQueueName);

            //do the waiting
            while (true)
            {
                //wait until each worker finishes
                foreach (var (activeWorkerTask, _, _) in _activeWorkerTasks)
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

                //get the worker starting info
                var workerStartingInformation = new RwStartingInformation
                {
                    Siw = new SiwInformation
                    {
                        CameraIp = cameraIp,
                        CameraPort = cameraPort
                    },
                    NotProcessedRequests = _activeRequests
                };

                //get the worker task
                var workerTask = remoteWorker
                    .ConfigureWorkersAndStartAsync(workerStartingInformation);

                //attempt to create the worker
                _activeWorkerTasks.Add((workerTask, remoteWorker, workerStartingInformation));
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
