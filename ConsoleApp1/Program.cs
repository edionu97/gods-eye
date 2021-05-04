using System;
using EasyNetQ;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using GodsEye.RemoteWorker.Workers.Messages;
using GodsEye.RemoteWorker.Workers.Messages.Requests;
using GodsEye.Utility.Application.Helpers.Helpers.Hashing;
using GodsEye.Utility.Application.Config.Configuration.Sections.RabbitMq;

namespace ConsoleApp1
{
    public class Program
    {

        public static async Task Main(string[] args)
        {

            //destruct the message or throw exception
            var (username, password, host, port) = new RabbitMqConfig
            {
                Username = "admin",
                Password = "admin",
                Host = "127.0.0.1",
                Port = 5672
            };

            //create the queue
            
            var queue = RabbitHutch.CreateBus(
                new ConnectionConfiguration
                {
                    UserName = username,
                    Password = password,
                    Hosts = new List<HostConfiguration>
                    {
                        new HostConfiguration
                        {
                            Host = host,
                            Port = port
                        }
                    }
                },
                _ => { });


            await queue.PubSub.PublishAsync<IRequestResponseMessage>(new SearchForPersonMessage
            {
                MessageContent = await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\rob.txt")
            });


            //await queue.PubSub.SubscribeAsync<PersonFoundMessage>(
            //    StringConstants.SlaveToMasterBusQueueName,
            //    r =>
            //    {
            //        Console.WriteLine(r.EndTimeUtc + " " + r.StartTimeUtc + " " + r.IsFound);
            //    });

            //await queue.PubSub.PublishAsync<IRequestResponseMessage>(new StopSearchingForPersonMessage
            //{
            //    MessageId = StringContentHasherHelpers.GetChecksumOfStringContent(await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\rob.txt"))
            //});

            //var provider = new KeyBasicHashProvider();

            //provider.RegisterKey("app key");

            //var decryptor = new KeyBasedEncryptorDecryptor(provider);

            ////get the address and the port
            //var cameraIpAddress = IPAddress.Parse("192.168.0.101");
            //var cameraIpEndPoint = new IPEndPoint(cameraIpAddress, 5001);

            //using var s = new Socket(cameraIpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            //{
            //    Blocking = true
            //};

            //s.Connect(cameraIpEndPoint);

            //while (true)
            //{
            //    var message = 
            //        await SendHelpers.ReceiveMessageAsync<NetworkImageFrameMessage>(s, decryptor);

            //    Console.WriteLine(message.FrameName);
            //}
            Console.ReadLine();
        }
    }
}
