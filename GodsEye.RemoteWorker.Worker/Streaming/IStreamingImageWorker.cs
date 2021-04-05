using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer;

namespace GodsEye.RemoteWorker.Worker.Streaming
{
    public interface IStreamingImageWorker
    {
        /// <summary>
        /// Gets the frame buffer
        /// </summary>
        public IFrameBuffer FrameBuffer { get; }

        /// <summary>
        /// Starts the worker
        /// </summary>
        /// <returns>a task to the worker</returns>
        public Task StartAsync(int cameraPort, string cameraAddress);
    }
}
