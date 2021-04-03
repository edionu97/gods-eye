using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using EasyNetQ;


namespace ConsoleApp1
{
    class Message
    {
        public string Msg { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {

            using (var bus = RabbitHutch.CreateBus(
                new ConnectionConfiguration
                {
                    UserName = "admin",
                    Password = "admin",
                    Hosts = new List<HostConfiguration>
                    {
                        new HostConfiguration
                        {
                            Host = "192.168.0.101",
                            Port = 5672
                        }
                    }
                },
                x =>
                {

                }))
            {
                bus.PubSub.Subscribe<Message>("test", (m) =>
                {

                });

                bus.PubSub.Subscribe<Message>("test2", (m) =>
                {

                });

                Console.ReadLine();
            }

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.PubSub.Publish(new Message
                {
                    Msg = "ana are mere"
                });

                bus.PubSub.Publish(new Message
                {
                    Msg = "ana are mere"
                });

                bus.PubSub.Publish(new Message
                {
                    Msg = "ana are mere"
                });

                bus.PubSub.Publish(new Message
                {
                    Msg = "ana are mere"
                });

                bus.PubSub.Publish(new Message
                {
                    Msg = "ana are mere"
                });

                bus.PubSub.Publish(new Message
                {
                    Msg = "ana are mere"
                });
            }




            ////var provider = new KeyBasicHashProvider();

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

        }
    }
}
