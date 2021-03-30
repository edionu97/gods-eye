using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GodsEye.Camera.ImageStreaming.Messages;
using GodsEye.Utility.Application.Helpers.Helpers.Network;
using GodsEye.Utility.Application.Security.Encryption.Impl;
using GodsEye.Utility.Application.Security.KeyProvider.Impl;

namespace ConsoleApp2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var provider = new KeyBasicHashProvider();

            provider.RegisterKey("app key");

            var decryptor = new KeyBasedEncryptorDecryptor(provider);

            //get the address and the port
            var cameraIpAddress = IPAddress.Parse("192.168.0.101");
            var cameraIpEndPoint = new IPEndPoint(cameraIpAddress, 5001);

            using var s = new Socket(cameraIpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                Blocking = true
            };

            s.Connect(cameraIpEndPoint);

            while (true)
            {
                var message =
                    await SendHelpers.ReceiveMessageAsync<ImageFrameMessage>(s, decryptor);

                Console.WriteLine(message.FrameName);
            }

        }
    }
}
