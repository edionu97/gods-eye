using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.WebSocket.Server;
using GodsEye.Camera.ImageStreaming.Messages;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Config.Settings.Camera;
using GodsEye.Utility.Application.Helpers.Helpers.Network;
using GodsEye.Utility.Application.Config.Settings.RemoteWorker;

namespace GodsEye.RemoteWorker.Worker.Streaming.Impl
{
    public partial class StreamingImageWorker : IStreamingImageWorker
    {
        private readonly IWebSocketServer _webSocketServer;
        private readonly IEncryptorDecryptor _encryptorDecryptor;

        private readonly ICameraSettings _cameraSettings;
        private readonly IWebSocketSettings _webSocketSettings;

        private readonly ILoggerFactory _loggerFactory;

        public StreamingImageWorker(
            IWebSocketServer webSocketServer, 
            IEncryptorDecryptor encryptor,
            ICameraSettings cameraSettings,
            IWebSocketSettings webSocketSettings, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _encryptorDecryptor = encryptor;
            _cameraSettings = cameraSettings;
            _webSocketServer = webSocketServer;
            _webSocketSettings = webSocketSettings;
        }

        public Task StartWorkerAsync(int workerId)
        {
            //create an worker and schedule the work on the threadPool
            return Task.Run(async () =>
            {
                //create the logger
                var logger = _loggerFactory
                    .CreateLogger($"{nameof(StreamingImageWorker)}_{workerId}");

                //connect to camera
                using var tcpSocket = ConnectToCamera(logger, _cameraSettings);

                //start the ws service
                await StartWebSocketAsync(
                    _webSocketServer, _webSocketSettings, workerId, logger);

                //infinite loop
                while (true)
                {
                    //read the message
                    var message = await SendHelpers
                        .ReceiveMessageAsync<ImageFrameMessage>(tcpSocket, _encryptorDecryptor);

                    //send the frame
                    await _webSocketServer.SendMessageAsync(message.FrameName);
                }

                // ReSharper disable once FunctionNeverReturns
            });
        }
        
    }
}
