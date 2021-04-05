using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Remote.StartingInfo;

namespace GodsEye.RemoteWorker.Worker.Remote
{
    public interface IRemoteWorker
    {
        /// <summary>
        /// Configure all the workers and start
        /// </summary>
        /// <param name="startingInformation">the information used for starting</param>
        public Task ConfigureWorkersAndStartAsync(StartingInformation startingInformation);
    }
}
