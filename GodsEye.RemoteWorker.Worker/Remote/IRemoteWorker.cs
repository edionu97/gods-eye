using System.Threading.Tasks;
using GodsEye.RemoteWorker.Workers.Messages;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;

namespace GodsEye.RemoteWorker.Worker.Remote
{
    public interface IRemoteWorker
    {
        /// <summary>
        /// Configure all the workers and start
        /// </summary>
        /// <param name="rwStartingInformation">the information used for starting</param>
        public Task ConfigureWorkersAndStartAsync(RwStartingInformation rwStartingInformation);

        /// <summary>
        /// This method is for informing the worker that is possible to have new requests
        /// </summary>
        /// <param name="requestMessage">the request message</param>
        /// <param name="rwStartingInformation">the worker's starting information</param>
        public Task CheckForNewRequestAsync(
            IRequestResponseMessage requestMessage, RwStartingInformation rwStartingInformation);
    }
}
