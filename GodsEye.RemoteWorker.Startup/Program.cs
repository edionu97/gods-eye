using System.Collections.Generic;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.Streaming;
using Microsoft.Extensions.DependencyInjection;

namespace GodsEye.RemoteWorker.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //get the service provider
            var service = RemoteWorkerBootstrapper.Load();

            var list = new List<Task>();
            for (var i = 0; i < 2; ++i)
            {
                //get the web socket server
                var imageWorker = service.GetService<IStreamingImageWorker>();

                if (imageWorker == null)
                {
                    continue;
                }

                list.Add(imageWorker.StartAsync(i));
            }

            Task.WaitAll(list.ToArray());

        }
    }
}
