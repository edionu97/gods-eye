using System.Threading.Tasks;
using GodsEye.RemoteWorker.Startup.Config;
using GodsEye.RemoteWorker.Worker.Coordinator;
using Microsoft.Extensions.DependencyInjection;

namespace GodsEye.RemoteWorker.Startup
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //get the service provider
            var service = RemoteWorkerBootstrapper.Load();

            //get the worker service
            var workerService = service
                .GetService<IRemoteWorkerCoordinatorStarter>();

            //treat the null case
            if (workerService == null)
            {
                return;
            }

            //await to finish
            await workerService.StartAsync();
        }
    }
}
