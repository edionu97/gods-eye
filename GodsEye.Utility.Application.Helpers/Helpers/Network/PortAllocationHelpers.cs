using System.Net;
using System.Net.Sockets;

namespace GodsEye.Utility.Application.Helpers.Helpers.Network
{
    public class PortAllocationHelpers
    {
        /// <summary>
        /// Get the next tcp available port 
        /// </summary>
        /// <returns></returns>
        public static int GetNextTcpAvailablePort()
        {
            //create a socket
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //connect to that endpoint
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));

            //get the port
            return ((IPEndPoint)socket.LocalEndPoint).Port;
        }
    }
}
