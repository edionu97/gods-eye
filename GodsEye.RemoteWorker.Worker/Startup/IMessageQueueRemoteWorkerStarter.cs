using System.Threading.Tasks;

namespace GodsEye.RemoteWorker.Worker.Startup
{
    public interface IMessageQueueRemoteWorkerStarter
    {
        /// <summary>
        /// Starts the worker
        /// </summary>
        public Task StartAsync();
    }
}
