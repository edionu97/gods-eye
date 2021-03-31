using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.Utility.Application.Config.Settings;
using WatsonWebsocket;

namespace GodsEye.RemoteWorker.Startup
{
    public class Program
    {
        //static IList<string> clients = new List<string>();

        //static void ClientConnected(object sender, ClientConnectedEventArgs args)
        //{
        //    Console.WriteLine("Client connected: " + args.IpPort);

        //    clients.Add(args.IpPort);

        //}

        //static void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        //{
        //    Console.WriteLine("Client disconnected: " + args.IpPort);
        //}

        //static void MessageReceived(object sender, MessageReceivedEventArgs args)
        //{
        //    Console.WriteLine("Message received from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
        //}

        public static async Task Main(string[] args)
        {

            var service = RemoteWorkerBootstrapper.Load();

            var a = service.GetService<IRemoteWorkerSettings>();

            var s = a.ServerAddress;
            //var server = new WatsonWsServer("localhost", 9002);


            //server.ClientConnected += ClientConnected;
            //server.ClientDisconnected += ClientDisconnected;
            //server.MessageReceived += MessageReceived;

            //await server.StartAsync();

            //while (true)
            //{
            //    await Task.Delay(100);

            //    if (!clients.Any())
            //    {
            //        continue;
            //    }

            //    foreach (var client in clients)
            //    {
            //        await server.SendAsync(client, "ana are mere");
            //    }
            //}

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
            //        await SendHelpers.ReceiveMessageAsync<ImageFrameMessage>(s, decryptor);

            //    Console.WriteLine(message.FrameName);
            //}

        }
    }
}
