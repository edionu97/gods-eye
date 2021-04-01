using System.Threading.Tasks;

namespace GodsEye.RemoteWorker.Worker.Streaming.WebSocket
{
    public interface IWebSocketServer
    {
        public bool IsServerListening { get; }

        /// <summary>
        /// Configure the ws socket server to starts on a specific port and address
        /// </summary>
        /// <param name="address">the address of the server</param>
        /// <param name="port">the value of the port</param>
        /// <returns>a task</returns>
        public Task ConfigureAsync(string address, int port);

        /// <summary>
        /// This method it is used to send a message to a specific client
        /// </summary>
        /// <param name="message">the value of the message</param>
        /// <param name="clientId">the id of the client (optional)</param>
        public Task SendMessageAsync<T>(T message, string clientId = null);
    }
}
