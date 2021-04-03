using System.Threading.Tasks;

namespace GodsEye.RemoteWorker.Startup.StartupWorker
{
    public interface IMessageQueueRemoteWorkerStarter
    {
        /// <summary>
        /// Starts the worker
        /// </summary>
        public Task StartAsync();
    }
}
