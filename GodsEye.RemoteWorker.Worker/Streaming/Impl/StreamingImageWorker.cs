using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Helpers.Helpers.Network;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

namespace GodsEye.RemoteWorker.Worker.Streaming.Impl
{
    public partial class StreamingImageWorker : IStreamingImageWorker
    {
        private readonly IConfig _appConfig;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebSocketServer _webSocketServer;
        private readonly IEncryptorDecryptor _encryptorDecryptor;

        public StreamingImageWorker(
            IWebSocketServer webSocketServer,
            IEncryptorDecryptor encryptor, 
            ILoggerFactory loggerFactory,
            IConfig appConfig)
        {
            _appConfig = appConfig;
            _loggerFactory = loggerFactory;
            _encryptorDecryptor = encryptor;
            _webSocketServer = webSocketServer;
        }

        public Task StartAsync(int workerId)
        {
            //create an worker and schedule the work on the threadPool
            return Task.Run(async () =>
            {
                //create the logger
                var logger = _loggerFactory
                    .CreateLogger($"{nameof(StreamingImageWorker)}_{workerId}");

                //connect to camera
                using var tcpSocket = ConnectToCamera(
                    logger,
                    _appConfig.Get<NetworkSectionConfig>(), workerId);

                //start the ws service
                await StartWebSocketAsync(
                    _webSocketServer, 
                    _appConfig.Get<WebSocketSectionConfig>(), workerId, logger);

                //get frames
                await foreach (var frame in GetFramesAsync(tcpSocket))
                {
                    //send the frame
                    await _webSocketServer.SendMessageAsync(frame);
                }
            });
        }

        private async IAsyncEnumerable<NetworkImageFrameMessage> GetFramesAsync(Socket fromCamera)
        {
            while (true)
            {
                yield return await SendHelpers
                    .ReceiveMessageAsync<NetworkImageFrameMessage>(fromCamera, _encryptorDecryptor);
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }
}
