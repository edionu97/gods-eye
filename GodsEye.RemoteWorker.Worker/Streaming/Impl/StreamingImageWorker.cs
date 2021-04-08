using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer;
using Microsoft.Extensions.Logging;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Helpers.Helpers.Network;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

using WorkerConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.Workers;

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
            IConfig appConfig,
            IFrameBuffer frameBuffer)
        {
            _appConfig = appConfig;
            _loggerFactory = loggerFactory;
            _encryptorDecryptor = encryptor;
            _webSocketServer = webSocketServer;
            FrameBuffer = frameBuffer;
        }

        public IFrameBuffer FrameBuffer { get; }

        public Task StartAsync(
            int cameraPort,
            string cameraAddress, CancellationTokenSource cancellationTokenSource)
        {
            //get the address and port of this worker process
            var remoteConfig = 
                _appConfig.Get<RemoteWorkerSectionConfig>();

            //create an worker and schedule the work on the threadPool
            return Task.Run(async () =>
            {
                //create the logger
                var logger = _loggerFactory
                    .CreateLogger($"{nameof(StreamingImageWorker)} => ({cameraAddress}:{cameraPort})");

                //connect to the camera
                try
                {
                    //connect to camera
                    using var tcpSocket =
                        ConnectToCamera(logger, cameraAddress, cameraPort);

                    //start the ws service
                    await StartWebSocketAsync(
                        _webSocketServer,
                        remoteConfig.WorkersAddress,
                        PortAllocationHelpers.GetNextTcpAvailablePort(), logger);

                    //get frames
                    await foreach (var frame in GetFramesAsync(tcpSocket))
                    {
                        //push the frame into frame buffer
                        FrameBuffer.PushFrame(frame);

                        //send the frame
                        await _webSocketServer.SendMessageAsync(frame);
                    }
                }
                catch (Exception e)
                {
                    using (logger.BeginScope(e.GetType()))
                    {
                        //log the exception
                        logger.LogCritical(
                            WorkerConstants.ConnectionStatusFailedMessage, cameraAddress, cameraPort);
                    }

                    //signal the cancel to all the other workers
                    cancellationTokenSource.Cancel();

                    //throw the exception
                    throw new Exception($"{cameraAddress}:{cameraPort}");
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
