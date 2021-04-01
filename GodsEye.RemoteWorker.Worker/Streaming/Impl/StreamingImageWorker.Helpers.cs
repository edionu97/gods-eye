using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using WorkerConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Worker.Streaming.Impl
{
    public partial class StreamingImageWorker
    {
        /// <summary>
        /// Connect to camera
        /// </summary>
        /// <param name="logger">the logger</param>
        /// <param name="networkConfig">the camera setting</param>
        /// <returns>returns the socket</returns>
        private static Socket ConnectToCamera(ILogger logger, NetworkSectionConfig networkConfig)
        {
            //deconstruct the object
            var (_, port, address) = networkConfig;

            //get the address and the port
            var cameraIpAddress = IPAddress.Parse(address);
            var cameraIpEndPoint = new IPEndPoint(cameraIpAddress, port);

            //declare the socket
            var tcpSocket = new Socket(cameraIpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                Blocking = true
            };

            //try to connect
            try
            {
                //log the message
                logger
                    .LogInformation(
                        WorkerConstants.TryingToConnectOnCameraMessage,
                        address, port);

                //connect to the camera
                tcpSocket.Connect(cameraIpEndPoint);

                //log successful message
                logger.LogInformation(WorkerConstants.ConnectionStatusSuccessfulMessage);
            }
            catch (Exception e)
            {
                //log error message
                logger.LogError(e, WorkerConstants.ConnectionStatusFailedMessage);
                throw;
            }

            return tcpSocket;
        }

        /// <summary>
        /// This will start the websocket instance on the port from the settingsAddress:settings + offset 
        /// </summary>
        /// <param name="webSocketServer">the websocket service</param>
        /// <param name="wsSettings">websocket settings</param>
        /// <param name="portOffset">the offset of the port</param>
        /// <param name="logger">the logger</param>
        private static async Task StartWebSocketAsync(
            IWebSocketServer webSocketServer,
            WebSocketSectionConfig wsSettings, int portOffset, ILogger logger = null)
        {
            //get the properties
            var (port, address) = wsSettings;

            //start the web socket server using the same port and address
            await webSocketServer.ConfigureAsync(address, port + portOffset);

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
