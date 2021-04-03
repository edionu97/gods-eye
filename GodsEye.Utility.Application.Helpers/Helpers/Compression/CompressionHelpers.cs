using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace GodsEye.Utility.Application.Helpers.Helpers.Compression
{
    public static class CompressionHelpers
    {
        /// <summary>
        /// Compresses a byte array
        /// </summary>
        /// <param name="data">data to compress</param>
        /// <returns>a byte array</returns>
        public static async Task<byte[]> CompressByteArrayAsync(byte[] data)
        {
            //treat the null case
            if (data == null)
            {
                return default;
            }

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

        /// <summary>
        /// Decompresses the already compressed byte array
        /// </summary>
        /// <param name="data">the data to decompress</param>
        /// <returns>a byte array representing the decompressed values</returns>
        public static async Task<byte[]> DecompressByteArrayAsync(byte[] data)
        {
            //treat the null case
            if (data == null)
            {
                return default;
            }

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
