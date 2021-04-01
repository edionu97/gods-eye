using WatsonWebsocket;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;

namespace GodsEye.RemoteWorker.Worker.Streaming.WebSocket.Impl
{
    public class JsonBroadcastWebSocketServer : IWebSocketServer
    {
        private WatsonWsServer _wsServer;

        public bool IsServerListening => (_wsServer?.IsListening).GetValueOrDefault();

        public async Task ConfigureAsync(string address, int port)
        {
            //create the ws server
            _wsServer = new WatsonWsServer(address, port);

            //start the ws
            await _wsServer.StartAsync();
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
    }
}
