using WatsonWebsocket;
using System.Threading.Tasks;

namespace GodsEye.RemoteWorker.WebSocket.Server.Impl
{
    public class BroadcastToAllClientsWebSocketServer : IWebSocketServer
    {
        private WatsonWsServer _wsServer;

        public bool IsServerListening => (_wsServer?.IsListening).GetValueOrDefault();

        public async Task StartAsync(string address, int port)
        {
            //create the ws server
            _wsServer = new WatsonWsServer(address, port);

            //start the ws
            await _wsServer.StartAsync();
        }

        public async Task SendMessageAsync(string message, string _ = null)
        {
            //iterate the clients
            foreach (var client in _wsServer.ListClients())
            {
                //send the message
                await _wsServer.SendAsync(client, message);
            }
        }
    }
}
