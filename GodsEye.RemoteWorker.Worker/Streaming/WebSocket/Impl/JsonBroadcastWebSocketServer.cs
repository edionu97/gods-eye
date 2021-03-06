using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using WatsonWebsocket;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Constants.String;
using GodsEye.Utility.Application.Resources.Manager;
using GodsEye.Utility.Application.Resources.Options;
using Microsoft.Extensions.Logging;

using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Ws;



namespace GodsEye.RemoteWorker.Worker.Streaming.WebSocket.Impl
{
    public class JsonBroadcastWebSocketServer : IWebSocketServer
    {
        private readonly ResWsClientConfig _wsClientConfig;
        private readonly IResourcesManager _resourcesManager;
        private readonly ILogger<JsonBroadcastWebSocketServer> _logger;

        private WatsonWsServer _wsServer;

        public bool IsServerListening => (_wsServer?.IsListening).GetValueOrDefault();

        public JsonBroadcastWebSocketServer(
            IConfig config,
            IResourcesManager resourcesManager,
            ILogger<JsonBroadcastWebSocketServer> logger)
        {
            _logger = logger;
            _resourcesManager = resourcesManager;
            _wsClientConfig = config.Get<ResWsClientConfig>();
        }

        public async Task ConfigureAsync(string address, int port)
        {
            //create the ws server
            _wsServer = new WatsonWsServer(address, port);

            //start the ws
            await _wsServer.StartAsync();

            //generate the client for ws
            await GenerateClient(address, port);
        }

        public async Task SendMessageAsync<T>(T message, string _ = null)
        {
            //iterate the clients
            foreach (var client in _wsServer.ListClients())
            {
                //send the message
                await _wsServer.SendAsync(
                    client, JsonSerializerDeserializer<T>.Serialize(message));
            }
        }

        private async Task GenerateClient(string address, int port)
        {
            try
            {
                //create a new logging scope
                using (_logger.BeginScope(Constants.AutoGeneratedClientMessage))
                {
                    //generate the client if requested
                    var (clientLocation, templateLocation, shouldGenerate) = _wsClientConfig;
                    if (shouldGenerate)
                    {
                        //path of new file
                        var pathOfNewFile = Path.Combine(clientLocation,
                            string.Format(StringConstants.Names.GeneratedWsClientNameFormat, address, port));

                        //create the generation options
                        var generationOptions = new TemplateGenerationOptions
                        {
                            GroupId = "placeholder",
                            PlaceholderIdentifier = new Regex(@"\$(?<placeholder>ws-server-address)\$"),
                            TemplateLocation = new FileInfo(templateLocation),
                            PlaceholdersValues = new Dictionary<string, string>
                            {
                                ["ws-server-address"] = $"{address}:{port}"
                            }
                        };

                        //create the file
                        var file = await _resourcesManager
                            .GenerateTemplateBasedResourceAsync(pathOfNewFile, generationOptions);

                        //log the message
                        _logger
                            .LogInformation(Constants.ClientWasAutoGeneratedSuccessfullyMessage, file.FullName);
                    }
                }
            }
            catch (Exception e)
            {
                //log the message
                using (_logger.BeginScope(e.Message))
                {
                    //log the warning message
                    _logger
                        .LogWarning(Constants.ClientCouldNotBeGeneratedMessage, address, port);
                }
            }
        }
    }
}
