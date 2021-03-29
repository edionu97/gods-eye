using System;
using System.Text;
using System.Net.Sockets;
using GodsEye.Utility.Helpers.Network.Message;
using GodsEye.Utility.Helpers.Serializers.JsonSerializer;

namespace GodsEye.Utility.Helpers.Network
{
    public class SendHelpers
    {

        /// <summary>
        /// This method it is used for converting a message into a byte buffer and send it over the network
        ///     The first 4 bytes represents the message length and the others represents the message
        ///     The first 4 bytes are in big endian format so little endian conversion it is needed
        /// </summary>
        /// <param name="message">the message that will be send</param>
        /// <param name="toSocket">the destination</param>
        public static void SendMessage<T>(
            INetworkMessage message, Socket toSocket) where T : class, INetworkMessage
        {
            //serialize the object
            var serializedObject =
                JsonSerializerDeserializer<T>.Serialize(message as T);

            //convert the string into an array of bytes
            var frameInfoBytes = Encoding.ASCII.GetBytes(serializedObject);

            //get the message length
            var len = frameInfoBytes.Length;

            //reverse the bytes if little endian
            var lenAsBigEndian = BitConverter.GetBytes(len);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenAsBigEndian);
            }

            //send the length of the message
            toSocket.Send(lenAsBigEndian);

            //send the message
            toSocket.Send(frameInfoBytes);
        }

        /// <summary>
        /// Converts a byte buffer into message.
        ///     The first 4 bytes represents the message length and the others represents the message
        ///     The first 4 bytes are in big endian format so little endian conversion it is needed
        /// </summary>
        /// <param name="fromSocket">the socket from which we are waiting to read a message</param>
        /// <returns>an instance of message</returns>
        public static T ReceiveMessage<T>(Socket fromSocket) where T : class, INetworkMessage
        {
            //get the number of bytes
            var lenByteArray = new byte[4];
            fromSocket.Receive(lenByteArray);

            //convert to machine endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenByteArray);
            }

            //get the message length
            var length = BitConverter
                .ToInt32(lenByteArray, 0);

            //receive the message bytes
            var messageBytes = new byte[length];
            fromSocket.Receive(messageBytes);

            //convert the array of bytes into a string
            var frameInfoBytes = Encoding.ASCII.GetString(messageBytes);

            //get the object
            return JsonSerializerDeserializer<T>.Deserialize(frameInfoBytes);
        }
    }
}
