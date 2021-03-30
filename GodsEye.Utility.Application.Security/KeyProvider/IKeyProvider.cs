namespace GodsEye.Utility.Application.Security.KeyProvider
{
    public interface IKeyProvider
    {
        /// <summary>
        /// Register a key
        /// </summary>
        /// <param name="key">the key that will be registered</param>
        public void RegisterKey(string key);

        /// <summary>
        /// Get the value of the key
        /// </summary>
        /// <returns>the value of the key</returns>
        public string GetKey();
    }
}
