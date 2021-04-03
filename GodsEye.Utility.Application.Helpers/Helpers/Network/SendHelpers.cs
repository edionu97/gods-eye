﻿using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Messages;

namespace GodsEye.Utility.Application.Helpers.Helpers.Network
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
                await Compress(
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
                await Decompress(
                    ReceiveExactNumberOfBytes(length, fromSocket));

            //convert the array of bytes into a string
            var receivedMessage = decryptor != null
                ? await decryptor.DecryptAsync(messageBytes, key)
                : Encoding.ASCII.GetString(messageBytes);

            //get the object
            return JsonSerializerDeserializer<T>.Deserialize(receivedMessage);
        }

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

        private static async Task<byte[]> Compress(byte[] data)
        {
            //store the result of the compressed stream
            await using var compressedStream = new MemoryStream();

            //get the compression stream
            await using var zipStream = new GZipStream(compressedStream, CompressionMode.Compress);

            //write values to compress that stream
            await zipStream.WriteAsync(data, 0, data.Length);

            //do the flush (write all the bytes in the compressed stream)
            await zipStream.FlushAsync();

            //return the compressed values
            return compressedStream.ToArray();
        }

        private static async Task<byte[]> Decompress(byte[] data)
        {
            //initialize the compressed stream
            await using var compressedStream = new MemoryStream(data);

            //create the zip stream for decompressing
            await using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);

            //create the decompressed stream
            await using var decompressedStream = new MemoryStream();

            //decompress the values
            await zipStream.CopyToAsync(decompressedStream);

            //get the decompressed values
            return decompressedStream.ToArray();
        }
    }
}
