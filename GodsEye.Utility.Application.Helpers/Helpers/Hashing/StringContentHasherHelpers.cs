using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using GodsEye.Utility.Application.Helpers.ExtensionMethods.PrimitivesExtensions;

using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.ContentHasherHelpers;

namespace GodsEye.Utility.Application.Helpers.Helpers.Hashing
{
    public static class StringContentHasherHelpers
    {
        /// <summary>
        /// Get the checksum of the content
        /// </summary>
        /// <param name="string">the string</param>
        /// <exception cref="ArgumentNullException">if the string is null or empty</exception>
        /// <returns>the checksum</returns>
        public static string GetChecksumOfStringContent(string @string)
        {
            //check if the string is eligible
            if (string.IsNullOrEmpty(@string))
            {
                throw new ArgumentException(Constants.StringNullOrEmptyMessage);
            }

            //convert the string into a stream
            using var stream = @string.AsStream();

            //get the checksum of the string
            return ComputeChecksumAsync(stream);
        }

        /// <summary>
        /// Helper method for computing the sha256 checksum over a stream of data
        /// </summary>
        /// <param name="stream">the stream containing the data</param>
        /// <returns>the checksum</returns>
        private static string ComputeChecksumAsync(Stream stream)
        {
            //create the instance of the sha algorithm
            using var sha256 = new SHA256Managed();

            //compute the checksum and return it
            var checksumBuilder = new StringBuilder();
            foreach (var @byte in sha256.ComputeHash(stream))
            {
                checksumBuilder.Append(@byte.ToString("x2"));
            }
            return checksumBuilder.ToString();
        }
    }
}
