using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Security.KeyProvider;

namespace GodsEye.Utility.Application.Security.Encryption.Impl
{
    public class KeyBasedEncryptorDecryptor : IEncryptorDecryptor
    {
        private readonly byte[] _salt;
        private readonly IKeyProvider _provider;

        public KeyBasedEncryptorDecryptor(IKeyProvider keyProvider)
        {
            //create the salt
            _salt = new byte[]
            {
                0x40, 0x80, 0x10, 0x15, 0x32, 0x21, 0x14, 0x64, 0x65, 0x76
            };

            //key provider
            _provider = keyProvider;
        }

        public async Task<byte[]> EncryptAsync(string dataToEncrypt, string key = null)
        {
            //declare the symmetric algorithm
            using var symmetricAlg =
                InitializeAlgorithm(key ?? _provider?.GetKey());

            //declare the memory stream
            await using var encryptedResultMemoryStream = new MemoryStream();

            //declare the crypto stream
            await using var cryptoStream = new CryptoStream(
                encryptedResultMemoryStream,
                symmetricAlg.CreateEncryptor(), CryptoStreamMode.Write);

            //transform the string in bytes and send into stream
            var dataToEncryptBytes = Encoding.Unicode.GetBytes(dataToEncrypt);
            await cryptoStream.WriteAsync(
                dataToEncryptBytes, 0, dataToEncryptBytes.Length);

            //finalize the encryption process
            cryptoStream.FlushFinalBlock();

            //get the bytes
            return encryptedResultMemoryStream.ToArray();
        }

        public async Task<string> DecryptAsync(byte[] dataToDecrypt, string key = null)
        {
            //declare the symmetric algorithm
            using var symmetricAlg =
                InitializeAlgorithm(key ?? _provider?.GetKey());

            //declare the memory stream
            await using var encryptedResultMemoryStream = new MemoryStream();

            //declare the crypto stream
            await using var cryptoStream = new CryptoStream(
                encryptedResultMemoryStream,
                symmetricAlg.CreateDecryptor(), CryptoStreamMode.Write);

            //transform the string in bytes and send into stream
            await cryptoStream.WriteAsync(
                dataToDecrypt, 0, dataToDecrypt.Length);

            //finalize the encryption process
            cryptoStream.FlushFinalBlock();

            //get the bytes
            return Encoding.Unicode.GetString(encryptedResultMemoryStream.ToArray());
        }

        private Rijndael InitializeAlgorithm(string password)
        {
            //get the password derivative bytes
            var derivativeBytes =
                new Rfc2898DeriveBytes(password, _salt);

            //declare the symmetric algorithm
            var symmetricAlg = Rijndael.Create();

            //set the key and the initialization vector
            //key must have 256 bytes 
            //iv  must have 128 bytes
            symmetricAlg.Key = derivativeBytes.GetBytes(32);
            symmetricAlg.IV = derivativeBytes.GetBytes(16);

            symmetricAlg.Padding = PaddingMode.Zeros;

            //return the algorithm
            return symmetricAlg;
        }
    }
}
