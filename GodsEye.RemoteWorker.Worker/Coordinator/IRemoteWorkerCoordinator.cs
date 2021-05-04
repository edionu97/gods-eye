using System.Threading.Tasks;

namespace GodsEye.RemoteWorker.Worker.Coordinator
{
    public interface IRemoteWorkerCoordinatorStarter
    {
        /// <summary>
        /// Starts the worker
        /// </summary>
        public Task StartAsync();
    }
}
