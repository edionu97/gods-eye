using System;
using System.IO;
using ConsoleApp1.Config;
using System.Threading.Tasks;
using GodsEye.Application.Middleware;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Workers.Messages.Responses;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Geolocation.Model;

namespace ConsoleApp1
{
    public class Program
    {
        private static readonly object SyncPrimitive = new object();

        public static async Task Main(string[] args)
        {
            //get the service provider
            var serviceProvider = Bootstrapper.Load();

            //get the message queue
            var workersMaster =
                serviceProvider
                    .GetService<IWorkersMasterMiddleware>()
                ?? throw new ArgumentNullException();

            //set the message callback
            await workersMaster.SetTheMessageCallbackAsync((r) =>
            {
                Console.WriteLine($"For user id {r.UserId}");
                Console.WriteLine($"Received {r.GetType().Name} with id {r.MessageId}");

                //based on the response type do one of the following
                switch (r)
                {
                    case ActiveWorkerMessageResponse activeWorkerMessageResponse:
                        {

                            //serialize the message
                            Console.WriteLine(JsonSerializerDeserializer<GeolocationInfo>.Serialize(activeWorkerMessageResponse.Geolocation));

                            break;
                        }

                    case PersonFoundMessageResponse searchForPersonResponse:
                        {
                            Console.WriteLine(searchForPersonResponse.EndTimeUtc + " " + searchForPersonResponse.StartTimeUtc + " " + searchForPersonResponse.IsFound);

                            var (response, _, analysis) = searchForPersonResponse.MessageContent;
                            Console.WriteLine(analysis.ToString());

                            Console.WriteLine(JsonSerializerDeserializer<GeolocationInfo>.Serialize(searchForPersonResponse.FromLocation));

                            break;
                        }
                }

                Console.WriteLine("\n");

                return Task.CompletedTask;
            });

            //get the base64 image
            var searchedFaceBase64Img =
                await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\eduard.txt");


            await workersMaster.StartSearchingAsync("eduard", searchedFaceBase64Img);
            await workersMaster.StartSearchingAsync("onu", searchedFaceBase64Img);

            await workersMaster.PingWorkersAsync("eduard");
            await workersMaster.PingWorkersAsync("onu");

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            await workersMaster.StopSearchingAsync("eduard", searchedFaceBase64Img);
            await workersMaster.StopSearchingAsync("onu", searchedFaceBase64Img);
        }
    }
}
