using System;
using System.Security.Cryptography;
using System.Text;

namespace GodsEye.Utility.Application.Security.KeyProvider.Impl
{
    public class KeyBasicHashProvider : IKeyProvider
    {
        private string _key;

        public void RegisterKey(string key)
        {
            //if the key is null then ignore
            if (key == null)
            {
                return;
            }

            //compute the sha256
            using var sha256 = SHA256.Create();

            //get the key
            _key = Convert
                .ToBase64String(sha256.ComputeHash(Encoding.Unicode.GetBytes(key)));
        }

        public string GetKey()
        {
            return _key;
        }
    }
}
