using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket;
using WorkerConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Worker.Streaming.Impl
{
    public partial class StreamingImageWorker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cameraAddress"></param>
        /// <param name="cameraPort"></param>
        /// <returns></returns>
        private static Socket ConnectToCamera(
            ILogger logger, string cameraAddress, int cameraPort)

        {
            //get the address and the port
            var cameraIpAddress = IPAddress.Parse(cameraAddress);
            var cameraIpEndPoint = new IPEndPoint(cameraIpAddress, cameraPort);

            //declare the socket
            var tcpSocket = new Socket(cameraIpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                Blocking = true
            };

            //log the message
            logger
                .LogInformation(
                    WorkerConstants.TryingToConnectOnCameraMessage,
                    cameraAddress, cameraPort);

            //connect to the camera
            tcpSocket.Connect(cameraIpEndPoint);

            //log successful message
            logger.LogInformation(WorkerConstants.ConnectionStatusSuccessfulMessage);

            return tcpSocket;
        }

        /// <summary>
        /// This will start the websocket instance on the port from the settingsAddress:settings + offset 
        /// </summary>
        /// <param name="webSocketServer">the websocket service</param>
        /// <param name="address">the address on which the ws starts</param>
        /// <param name="port">the port used</param>
        /// <param name="logger">the logger</param>
        private static async Task StartWebSocketAsync(
            IWebSocketServer webSocketServer,
            string address, int port, ILogger logger = null)
        {
            //start the web socket server using the same port and address
            await webSocketServer.ConfigureAsync(address, port);

            //check if the web socket is listening
            if (!webSocketServer.IsServerListening)
            {
                logger?.LogWarning(WorkerConstants
                    .WebsocketServerCouldNotBeStartMessage);
                return;
            }

            //log the message
            logger?.LogInformation(WorkerConstants
                .WebSocketListeningOnPortMessage, address, port);
        }
    }
}
