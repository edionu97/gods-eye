using System;
using System.IO;
using System.Threading.Tasks;
using ConsoleApp1.Config;
using EasyNetQ;
using GodsEye.RemoteWorker.Worker.Remote.Messages;
using GodsEye.RemoteWorker.Workers.Messages;
using GodsEye.RemoteWorker.Workers.Messages.Requests;
using GodsEye.Utility.Application.Helpers.Helpers.Hashing;
using GodsEye.Utility.Application.Items.Constants.String;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Formats.Bmp;

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
            var messageQueue =
                serviceProvider
                    .GetService<IBus>()
                ?? throw new ArgumentNullException();

            //get the base64 image
            var searchedFaceBase64Img =
                await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\bica tanara.txt");

            //start the search for person message
            await messageQueue.PubSub.PublishAsync<IRequestResponseMessage>(new SearchForPersonMessage
            {
                MessageContent = searchedFaceBase64Img
            });

            const int numberOfCycles = 100;
            var runningCycles = 0;

            var averageTime = .0;
            var numberOfCasesFound = .0;

            //this is called when we have an response
            await messageQueue.PubSub.SubscribeAsync<PersonFoundMessage>(
                StringConstants.SlaveToMasterBusQueueName,
                async r =>
                {
                    Console.WriteLine(r.EndTimeUtc + " " + r.StartTimeUtc + " " + r.IsFound);

                    //sync the values
                    lock (SyncPrimitive)
                    {
                        //increment the number of times when the values were found
                        numberOfCasesFound += r.IsFound ? 1 : 0;

                        //get the difference in seconds
                        averageTime += (r.EndTimeUtc - r.StartTimeUtc).TotalMilliseconds;
                    }

                    //if the number of running cycles is less then max number of cycles do nothing
                    if (++runningCycles < numberOfCycles)
                    {
                        return;
                    }

                    //write the stats
                    Console.WriteLine($"The average time for running the values is {(averageTime / runningCycles) / 1000}s");
                    Console.WriteLine($"The success probability is {(numberOfCasesFound / runningCycles) * 100}%");

                    //send the cancellation request message
                    await messageQueue.PubSub.PublishAsync<IRequestResponseMessage>(new StopSearchingForPersonMessage
                    {
                        MessageId = StringContentHasherHelpers.GetChecksumOfStringContent(searchedFaceBase64Img)
                    });
                });


            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }
    }
}
