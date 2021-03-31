using System.Threading.Tasks;

namespace GodsEye.RemoteWorker.Worker.Streaming
{
    public interface IStreamingImageWorker
    {
        /// <summary>
        /// Starts the worker
        /// </summary>
        /// <returns>a task to the worker</returns>
        public Task StartWorkerAsync(int workerId);
    }
}
