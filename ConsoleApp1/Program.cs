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

            const int numberOfCycles = 10;
            var runningCycles = 0;

            //this is called when we have an response
            await messageQueue.PubSub.SubscribeAsync<PersonFoundMessage>(
                StringConstants.SlaveToMasterBusQueueName,
                async r =>
                {

                    Console.WriteLine(r.EndTimeUtc + " " + r.StartTimeUtc + " " + r.IsFound);


                    //if the number of running cycles is less then max number of cycles do nothing
                    if (++runningCycles < numberOfCycles)
                    {
                        return;
                    }

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
