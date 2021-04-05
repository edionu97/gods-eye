using System;
using System.Threading;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;

namespace GodsEye.RemoteWorker.Worker.Remote.Impl
{
    public class RemoteWorker : IRemoteWorker
    {
        private readonly IStreamingImageWorker _streamingImageWorker;

        public RemoteWorker(IStreamingImageWorker streamingImageWorker)
        {
            _streamingImageWorker = streamingImageWorker;
        }

        public async Task ConfigureWorkersAndStartAsync(StartingInformation startingInformation)
        {
            //get the starting information
            var (cameraIp, cameraPort) = startingInformation.Siw;

            var _ = Task.Run(ReadingTask);

            //start the siw worker
            await _streamingImageWorker.StartAsync(cameraPort, cameraIp);
        }

        public void ReadingTask()
        {
            var frameBuffer = _streamingImageWorker.FrameBuffer;
            while (true)
            {
                Thread.Sleep(100);

                var snapshot = frameBuffer.TakeASnapshot();

                Console.WriteLine(snapshot.Peek().Item1.Ticks);
            }
        }
    }
}
