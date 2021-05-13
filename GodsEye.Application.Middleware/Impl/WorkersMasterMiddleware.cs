using System;
using EasyNetQ;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Workers.Messages;
using GodsEye.RemoteWorker.Workers.Messages.Requests;
using GodsEye.RemoteWorker.Workers.Messages.Responses;
using GodsEye.Utility.Application.Helpers.Helpers.Hashing;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Constants.String;

namespace GodsEye.Application.Middleware.Impl
{
    public class WorkersMasterMiddleware : IWorkersMasterMiddleware
    {
        private readonly IBus _messageBus;

        public Func<IRequestResponseMessage, Task> OnMessageCallback { get; private set; }

        public WorkersMasterMiddleware(IBus messageBus)
        {
            _messageBus = messageBus;
        }

        public async Task SetTheMessageCallbackAsync(Func<IRequestResponseMessage, Task> callback)
        {
            //ignore null values
            if (callback == null)
            {
                return;
            }

            //set the message callback
            OnMessageCallback = callback;

            //register the active message response
            await _messageBus.PubSub
                .SubscribeAsync<ActiveWorkerMessageResponse>(
                    StringConstants.SlaveToMasterBusQueueName,
                    async m => await OnMessageCallback(m));

            //register the active message response
            await _messageBus.PubSub
                .SubscribeAsync<PersonFoundMessageResponse>(
                    StringConstants.SlaveToMasterBusQueueName,
                    async m => await OnMessageCallback(m));
        }

        public async Task PingWorkersAsync(string userId)
        {
            //create the message
            var message = new GetActiveWorkersMessageRequest
            {
                UserId = userId,
                MessageId = GetChecksumValue(userId, string.Empty)
            };

            //publish the message
            await _messageBus.PubSub
                .PublishAsync<IRequestResponseMessage>(message);
        }

        public async Task StartSearchingAsync(string userId, string searchedPerson)
        {
            //create the message
            var message = new SearchForPersonMessageRequest
            {
                UserId = userId,
                MessageContent = searchedPerson,
                MessageId = GetChecksumValue(userId, searchedPerson)
            };

            //publish the message
            await _messageBus.PubSub
                .PublishAsync<IRequestResponseMessage>(message);
        }

        public async Task StopSearchingAsync(string userId, string searchedPerson)
        {
            //create the message
            var message = new StopSearchingForPersonMessageRequest
            {
                UserId = userId,
                MessageId = GetChecksumValue(userId, searchedPerson)
            };

            //publish the message
            await _messageBus.PubSub
                .PublishAsync<IRequestResponseMessage>(message);
        }

        public string GetChecksumValue(string userId, string searchedPerson)
        {
            //get the hash object
            var hashedObject = JsonSerializerDeserializer<dynamic>.Serialize(new
            {
                Salt = userId,
                Content = searchedPerson
            });

            //compute the checksum
            return StringContentHasherHelpers.GetChecksumOfStringContent(hashedObject);
        }
    }
}
