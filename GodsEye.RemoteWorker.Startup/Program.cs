using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Startup;
using GodsEye.RemoteWorker.Startup.Config;
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
                .GetService<IMessageQueueRemoteWorkerStarter>();

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
