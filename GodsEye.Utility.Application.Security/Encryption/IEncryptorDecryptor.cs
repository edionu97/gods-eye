using System.Threading.Tasks;

namespace GodsEye.Utility.Application.Security.Encryption
{
    public interface IEncryptorDecryptor
    {
        /// <summary>
        /// EncryptAsync data and return the encrypted byte values
        /// </summary>
        /// <param name="dataToEncrypt">the input data</param>
        /// <param name="key">value used to encrypt</param>
        /// <returns>an array of bytes representing the encrypted bytes</returns>
        public Task<byte[]> EncryptAsync(string dataToEncrypt, string key = null);

        /// <summary>
        /// DecryptAsync data and return a specific instance of TData
        /// </summary>
        /// <param name="dataToDecrypt">the encrypted message</param>
        /// <param name="key">value used to decrypt</param>
        /// <returns>the decrypted message</returns>
        public Task<string> DecryptAsync(byte[] dataToDecrypt, string key = null);
    }
}
