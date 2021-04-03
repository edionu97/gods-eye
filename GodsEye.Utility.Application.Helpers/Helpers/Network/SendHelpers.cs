using System;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Items.Messages;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using static GodsEye.Utility.Application.Helpers.Helpers.Compression.CompressionHelpers;

namespace GodsEye.Utility.Application.Helpers.Helpers.Network
{
    public partial class SendHelpers
    {
        /// <summary>
        /// This method it is used for converting a message into a byte buffer and send it over the network
        ///     The first 4 bytes represents the message length and the others represents the message
        ///     The first 4 bytes are in big endian format so little endian conversion it is needed
        /// </summary>
        /// <param name="message">the message that will be send</param>
        /// <param name="toSocket">the destination</param>
        /// <param name="encryptor">if specified this will be used for encrypting the message</param>
        /// <param name="key">the key used to encrypt the message</param>
        public static async Task SendMessageAsync<T>(
            ISerializableByteNetworkMessage message,
            Socket toSocket,
            IEncryptorDecryptor encryptor = null, string key = null) where T : class, ISerializableByteNetworkMessage
        {
            //serialize the object
            var serializedObject =
                JsonSerializerDeserializer<T>.Serialize(message as T);

            //convert the string into an array of bytes
            //encrypt the message if needed
            var bytesToSend = 
                await CompressByteArrayAsync(
                    encryptor != null 
                        ? await encryptor.EncryptAsync(serializedObject, key) 
                        : Encoding.ASCII.GetBytes(serializedObject));

            //get the message length
            var len = bytesToSend.Length;

            //reverse the bytes if little endian
            var lenAsBigEndian = BitConverter.GetBytes(len);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenAsBigEndian);
            }

            //send the length of the message
            toSocket.Send(lenAsBigEndian);

            //send the message
            toSocket.Send(bytesToSend);
        }

        /// <summary>
        /// Converts a byte buffer into message.
        ///     The first 4 bytes represents the message length and the others represents the message
        ///     The first 4 bytes are in big endian format so little endian conversion it is needed
        /// </summary>
        /// <param name="fromSocket">the socket from which we are waiting to read a message</param>
        /// <param name="decryptor">this will be used to decrypt the message</param>
        /// <param name="key">the key used for decryption</param>
        /// <returns>an instance of message</returns>
        public static async Task<T> ReceiveMessageAsync<T>(
            Socket fromSocket,
            IEncryptorDecryptor decryptor = null, string key = null) where T : class, ISerializableByteNetworkMessage
        {
            //get the number of bytes
            var lenByteArray
                = ReceiveExactNumberOfBytes(4, fromSocket);

            //convert to machine endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenByteArray);
            }

            //get the message length
            var length = BitConverter
                .ToInt32(lenByteArray, 0);

            //receive the message bytes
            var messageBytes = 
                await DecompressByteArrayAsync(
                    ReceiveExactNumberOfBytes(length, fromSocket));

            //convert the array of bytes into a string
            var receivedMessage = decryptor != null
                ? await decryptor.DecryptAsync(messageBytes, key)
                : Encoding.ASCII.GetString(messageBytes);

            //get the object
            return JsonSerializerDeserializer<T>.Deserialize(receivedMessage);
        }
    }
}
