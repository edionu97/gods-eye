using System;
using WatsonWebsocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using GodsEye.RemoteWorker.Workers.Messages;

namespace GodsEye.Application.Middleware.MessageBroadcaster.Impl
{
    public partial class MessageBroadcasterMiddleware : IMessageBroadcasterMiddleware
    {
        private readonly WatsonWsServer _wsServer;

        private readonly ILogger<MessageBroadcasterMiddleware> _logger;

        private readonly ConcurrentDictionary<string, string> _wsSessionIdToUserId = new ConcurrentDictionary<string, string>();

        public MessageBroadcasterMiddleware(WatsonWsServer wsServer, ILogger<MessageBroadcasterMiddleware> logger)
        {
            //set the ws server
            _wsServer = wsServer;

            //set the logger
            _logger = logger;

            //register the handler for message received
            _wsServer.MessageReceived += OnMessageFromClient;
            _wsServer.ClientDisconnected += OnClientDisconnected;
        }

        public Task BroadcastMessageAsync(IRequestResponseMessage r)
        {
            try
            {
                //serialize the response
                var clientResponse = SerializeMessageInClientResponse(r);

                //iterate in the active session workers
                foreach (var (sessionId, userId) in _wsSessionIdToUserId)
                {
                    //ignore those that are not 
                    if (userId != r.UserId)
                    {
                        continue;
                    }

                    //send the message to the client
                    _wsServer.SendAsync(sessionId, clientResponse);
                }

            }
            catch (Exception)
            {
                //ignore
            }

            return Task.CompletedTask;
        }

    }
}
