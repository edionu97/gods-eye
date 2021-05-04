using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Remote.Messages;
using GodsEye.RemoteWorker.Worker.Remote.Messages.Requests;
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


        public Task CheckForNewRequestAsync(IMessage requestMessage, RwStartingInformation rwStartingInformation);
    }
}
