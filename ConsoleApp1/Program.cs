using System;
using EasyNetQ;
using System.IO;
using ConsoleApp1.Config;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Workers.Messages;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Workers.Messages.Requests;
using GodsEye.RemoteWorker.Workers.Messages.Responses;
using GodsEye.Utility.Application.Helpers.Helpers.Hashing;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Constants.String;
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
            var messageQueue =
                serviceProvider
                    .GetService<IBus>()
                ?? throw new ArgumentNullException();

            //get the base64 image
            var searchedFaceBase64Img =
                await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\eduard.txt");

            //get the hash value
            var hashValue = StringContentHasherHelpers.GetChecksumOfStringContent(searchedFaceBase64Img);

            //start the search for person message
            await messageQueue.PubSub.PublishAsync<IRequestResponseMessage>(new SearchForPersonMessageRequest
            {
                MessageContent = searchedFaceBase64Img
            });

            const int numberOfCycles = 100;
            var runningCycles = 0;

            var averageTime = .0;
            var numberOfCasesFound = .0;

            //this is called when we have an response
            await messageQueue.PubSub.SubscribeAsync<PersonFoundMessageResponse>(
                StringConstants.SlaveToMasterBusQueueName,
                async r =>
                {
                    //get the messages only for his request
                    if (r.MessageId != hashValue)
                    {
                        return;
                    }

                    Console.WriteLine(r.EndTimeUtc + " " + r.StartTimeUtc + " " + r.IsFound);

                    var (response, _, analysis) = r.MessageContent;
                    Console.WriteLine(analysis.ToString());

                    Console.WriteLine(JsonSerializerDeserializer<GeolocationInfo>.Serialize(r.FromLocation));

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
                    await messageQueue.PubSub.PublishAsync<IRequestResponseMessage>(new StopSearchingForPersonMessageRequest
                    {
                        MessageId = StringContentHasherHelpers.GetChecksumOfStringContent(searchedFaceBase64Img)
                    });
                });

            await messageQueue.PubSub.SubscribeAsync<ActiveWorkerMessageResponse>(
                StringConstants.SlaveToMasterBusQueueName,
                m =>
                {
                    Console.WriteLine(m.MessageContent.Item1 + " " + m.MessageContent.Item2.Count );
                    Console.WriteLine(JsonSerializerDeserializer<GeolocationInfo>.Serialize(m.Geolocation));
                });


            await messageQueue.PubSub.PublishAsync<IRequestResponseMessage>(new GetActiveWorkersMessageRequest
            {
                MessageId = "this"
            });

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }
    }
}
