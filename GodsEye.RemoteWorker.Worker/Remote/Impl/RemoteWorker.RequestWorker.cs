using System;
using EasyNetQ;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Worker.FacialAnalysis;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.Utility.Application.Items.Constants.String;
using GodsEye.Utility.Application.Helpers.Helpers.Hashing;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;
using GodsEye.Utility.Application.Items.Messages.MasterToSlave;
using GodsEye.Utility.Application.Items.Messages.MasterToSlave.Impl.Requests;
using GodsEye.Utility.Application.Items.Messages.SlaveToMaster.Impl.Responses;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public partial class RemoteWorker
    {
        private readonly ConcurrentDictionary<string, (Task<SearchForPersonResponse>, CancellationTokenSource)> _onlineRecognitionWorkers =
            new ConcurrentDictionary<string, (Task<SearchForPersonResponse>, CancellationTokenSource)>();

        private async Task RegisterHandlersAsync(string cameraIp, int cameraPort)
        {
            //create a new subscription id
            var subscriptionId = Guid.NewGuid();

            //get the cancellation token
            var cancellationToken = _parentCancellationTokenSource.Token;

            //start listening for search for person messages
            _subscriptionResults.Add(
                await _messageBus.PubSub
                .SubscribeAsync<SearchForPersonMessage>(
                    StringConstants.MasterToSlaveBusQueueName + subscriptionId,
                    async r => await OnSearchForPersonMessageAsync(r, cameraIp, cameraPort, cancellationToken),
                    cancellationToken));

            //start listening for cancellation messages
            _subscriptionResults.Add(
            await _messageBus.PubSub
                .SubscribeAsync<StopSearchingForPersonMessage>(
                    StringConstants.MasterToSlaveBusQueueName + subscriptionId,
                     r => OnStopSearchingForPersonMessage(r, cameraIp, cameraPort),
                    cancellationToken));
        }

        private async Task OnSearchForPersonMessageAsync(SearchForPersonMessage message, string cameraIp, int cameraPort, CancellationToken token)
        {
            //set the identification number if not already set 
            message.IdentificationNumber ??= await StringContentHasherHelpers
                .GetChecksumOfStringContentAsync(message.MessageContent);

            //get the service
            var facialAnalysisWorkerInstance = 
                _serviceProvider
                    .GetService<IFacialAnalysisAndRecognitionWorker>()
                ?? throw new ArgumentNullException();

            //create a linked token source, relative to the parent
            //if child gets cancelled => parent is alive
            //if parent gets cancelled => all children are cancelled 
            var recognitionTaskCancellation =
                CancellationTokenSource.CreateLinkedTokenSource(token);

            //create a new recognition task
            var recognitionTask = facialAnalysisWorkerInstance
                .StartSearchingForPersonAsync(
                    new FarwStartingInformation
                    {
                        FrameBuffer = _streamingImageWorker.FrameBuffer,
                        StatisticsInformation = (cameraIp, cameraPort),
                        SearchedPersonBase64Img = message.MessageContent,
                        OnBufferProcessed = (r, startTime, endTime) =>
                       {
                           //send the response only if the value is not null
                           if (r == null)
                           {
                               //return;
                           }

                           //publish the result message async
                           //person potentially found
                           _messageBus.PubSub
                               // ReSharper disable once MethodSupportsCancellation
                               .Publish(new PersonFoundMessage<SearchForPersonResponse>
                               {
                                   IsFound = r != null,
                                   MessageContent = r,
                                   IdentificationNumber = message.IdentificationNumber,
                                   StartTimeUtc = startTime,
                                   EndTimeUtc = endTime
                               });
                       }
                    },
                    recognitionTaskCancellation.Token);

            //register the worker as online
            _onlineRecognitionWorkers
                .TryAdd(message.IdentificationNumber, (recognitionTask, recognitionTaskCancellation));
        }

        private void OnStopSearchingForPersonMessage(IMasterToSlaveMessage message, string cameraIp, int cameraPort)
        {
            //get the message id
            var id = message.IdentificationNumber;

            //handle the empty id case
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            //if there is no worker that handles that request return nothing
            if (!_onlineRecognitionWorkers.TryGetValue(id, out var recognitionDetails))
            {
                return;
            }

            //unpack the object
            var (_, cancellationTokenSource) = recognitionDetails;

            //cancel the recognition worker
            try
            {
                cancellationTokenSource.Cancel();
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
