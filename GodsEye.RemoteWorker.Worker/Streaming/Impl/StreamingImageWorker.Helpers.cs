using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.WebSocket.Server;
using GodsEye.Utility.Application.Config.Settings.Camera;
using GodsEye.Utility.Application.Config.Settings.RemoteWorker;
using WorkerConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

namespace GodsEye.RemoteWorker.Worker.Streaming.Impl
{
    public partial class StreamingImageWorker
    {
        /// <summary>
        /// Connect to camera
        /// </summary>
        /// <param name="logger">the logger</param>
        /// <param name="cameraSettings">the camera setting</param>
        /// <returns></returns>
        private static Socket ConnectToCamera(ILogger logger, ICameraSettings cameraSettings)
        {
            //get the address and the port
            var cameraIpAddress = IPAddress.Parse(cameraSettings.CameraAddress);
            var cameraIpEndPoint = new IPEndPoint(cameraIpAddress, cameraSettings.CameraStreamingPort);

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
                        cameraSettings.CameraAddress,
                        cameraSettings.CameraStreamingPort);

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
            IWebSocketSettings wsSettings, int portOffset, ILogger logger = null)
        {
            //get the properties
            var (port, address) = wsSettings;

            //start the web socket server using the same port and address
            await webSocketServer.StartAsync(address, port + portOffset);

            //check if the web socket is listening
            if (!webSocketServer.IsServerListening)
            {
                logger?.LogWarning(WorkerConstants
                    .WebsocketServerCouldNotBeStartMessage);
                return;
            }

            logger?.LogInformation(WorkerConstants
                .WebSocketListeningOnPortMessage, address, port);
        }
    }
}
