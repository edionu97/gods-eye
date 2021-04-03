using System.Net.Sockets;

namespace GodsEye.Utility.Application.Helpers.Helpers.Network
{
    public partial class SendHelpers
    {

        /// <summary>
        /// This function it is used for receiving an fixed number of bytes from a socket
        /// </summary>
        /// <param name="bytesToReceive">the number of bytes that is required to be received</param>
        /// <param name="client">the client socket</param>
        /// <returns>a byte buffer that represents the data to be read</returns>
        private static byte[] ReceiveExactNumberOfBytes(int bytesToReceive, Socket client)
        {
            //allocate the size in bytes
            var byteBuffer = new byte[bytesToReceive];

            //try to read the number of bytes in one shot
            var receivedBytes =
                client.Receive(byteBuffer, 0, bytesToReceive, SocketFlags.None);

            //if there are no items to read
            while (receivedBytes != bytesToReceive)
            {
                receivedBytes += client.Receive(
                    byteBuffer,
                    receivedBytes,
                    bytesToReceive - receivedBytes,
                    SocketFlags.None);
            }

            //return the bytes
            return byteBuffer;
        }
    }
}
