using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;
using GodsEye.Utility.Application.Config.BaseConfig;
using Microsoft.Extensions.Logging;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public class RemoteWorker : IRemoteWorker
    {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly IFacialRecognitionAndAnalysisProxy _proxy;
        private readonly IStreamingImageWorker _streamingImageWorker;

        private readonly ILoggerFactory _loggerFactory;

        public RemoteWorker(
            IStreamingImageWorker streamingImageWorker, 
            IFacialRecognitionAndAnalysisProxy proxy,
            ILoggerFactory loggerFactory)
        {
            _proxy = proxy;
            _streamingImageWorker = streamingImageWorker;
            _loggerFactory = loggerFactory;
            _cancellationToken = new CancellationTokenSource();
        }

        public async Task ConfigureWorkersAndStartAsync(StartingInformation startingInformation)
        {
            //get the starting information
            var (cameraIp, cameraPort) = startingInformation.Siw;

            var _ = Task.Run(() => ReadingTask(cameraPort, cameraIp));

            //start the siw worker
            await _streamingImageWorker
                .StartAsync(cameraPort, cameraIp, _cancellationToken);
        }

        public async Task ReadingTask(int port, string ip)
        {
            var logger = _loggerFactory.CreateLogger($"Aiw {ip}: {port}");

            //get the token
            var token = _cancellationToken.Token;

            var searchedPerson = await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\rob.txt", token);

            //get the frame buffer
            var frameBuffer = _streamingImageWorker.FrameBuffer;
            while (!token.IsCancellationRequested)
            {
                foreach (var f in frameBuffer.TakeASnapshot())
                {
                    try
                    {
                        var r = await _proxy.IdentifyPersonAsync(
                            f.Item2.ImageBase64EncodedBytes, searchedPerson
                        );

                        logger.LogInformation(r + " " + r.IsFound +"\n");
                    }
                    catch (Exception e)
                    {
                        logger.LogCritical(e.Message);
                    }
                }
            }


        }
    }
}
