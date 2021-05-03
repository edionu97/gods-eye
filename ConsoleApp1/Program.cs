using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Management.Client;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.RabbitMq;
using GodsEye.Utility.Application.Helpers.Helpers.Hashing;
using GodsEye.Utility.Application.Helpers.Helpers.Network;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Constants.String;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
using GodsEye.Utility.Application.Items.Messages.MasterToSlave;
using GodsEye.Utility.Application.Items.Messages.MasterToSlave.Impl.Requests;
using GodsEye.Utility.Application.Items.Messages.SlaveToMaster.Impl.Responses;
using GodsEye.Utility.Application.Security.Encryption.Impl;
using GodsEye.Utility.Application.Security.KeyProvider.Impl;


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


            await queue.PubSub.PublishAsync(new SearchForPersonMessage
            {
                MessageContent = await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\rob.txt")
            });


            await queue.PubSub.SubscribeAsync<PersonFoundMessage<SearchForPersonResponse>>(
                StringConstants.SlaveToMasterBusQueueName,
                r =>
                {
                    Console.WriteLine(r.EndTimeUtc +  " "  + r.StartTimeUtc + " " + r.IsFound);
                });

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
