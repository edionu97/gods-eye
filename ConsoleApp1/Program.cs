using System;
using System.IO;
using ConsoleApp1.Config;
using System.Threading.Tasks;
using GodsEye.Application.Middleware;
using GodsEye.Application.Middleware.MessageBroadcaster;
using GodsEye.Application.Middleware.WorkersMaster;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Workers.Messages.Responses;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Geolocation.Model;

namespace ConsoleApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //get the service provider
            var serviceProvider = Bootstrapper.Load();

            //get the message queue
            var workersMaster =
                serviceProvider
                    .GetService<IWorkersMasterMiddleware>()
                ?? throw new ArgumentNullException();


            //get the base64 image
            var searchedFaceBase64Img =
                await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\eduard.txt");


            //await workersMaster.StartSearchingAsync("eduard", searchedFaceBase64Img);
            //await workersMaster.StartSearchingAsync("onu", searchedFaceBase64Img);

            await workersMaster.PingWorkersAsync("eduard");
            await workersMaster.PingWorkersAsync("onu");

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            //await workersMaster.StopSearchingAsync("eduard", searchedFaceBase64Img);
            //await workersMaster.StopSearchingAsync("onu", searchedFaceBase64Img);
        }
    }
}
