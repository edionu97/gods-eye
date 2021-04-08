using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public class RemoteWorker : IRemoteWorker
    {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly IStreamingImageWorker _streamingImageWorker;

        public RemoteWorker(IStreamingImageWorker streamingImageWorker)
        {
            _streamingImageWorker = streamingImageWorker;
            _cancellationToken = new CancellationTokenSource();
        }

        public async Task ConfigureWorkersAndStartAsync(StartingInformation startingInformation)
        {
            //get the starting information
            var (cameraIp, cameraPort) = startingInformation.Siw;

            var _ = Task.Run(ReadingTask);

            //start the siw worker
            await _streamingImageWorker
                .StartAsync(cameraPort, cameraIp, _cancellationToken);
        }

        public async Task ReadingTask()
        {
            //get the token
            var token = _cancellationToken.Token;

            //get the frame buffer
            var frameBuffer = _streamingImageWorker.FrameBuffer;
            while (!token.IsCancellationRequested)
            {
                //await 1 second
                await Task.Delay(1000, token);

                //take the snapshot
                var snapshot = frameBuffer.TakeASnapshot();
                if (!snapshot.Any())
                {
                    continue;
                }

                Console.WriteLine(snapshot.Peek().Item1.Ticks);
            }

            
        }
    }
}
