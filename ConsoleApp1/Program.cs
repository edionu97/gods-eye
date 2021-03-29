using System;
using System.Net;
using System.Net.Sockets;
using GodsEye.Camera.ImageStreaming.Messages;
using GodsEye.Utility.Application.Helpers.Helpers.Network;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                var message = SendHelpers.ReceiveMessage<ImageFrameMessage>(s);

                Console.WriteLine(message.FrameName);
            }

        }
    }
}
