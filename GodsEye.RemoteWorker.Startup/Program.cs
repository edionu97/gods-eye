using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Streaming;
using Microsoft.Extensions.DependencyInjection;

namespace GodsEye.RemoteWorker.Startup
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //get the service provider
            var service = RemoteWorkerBootstrapper.Load();

            //get the web socket server
            var imageWorker = service.GetService<IStreamingImageWorker>();

            if (imageWorker == null)
            {
                return;
            }

            await imageWorker.StartWorkerAsync(0);
        }
    }
}
