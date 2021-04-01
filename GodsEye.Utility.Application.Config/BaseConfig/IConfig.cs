namespace GodsEye.Utility.Application.Config.BaseConfig
{
    public interface IConfig
    {
        /// <summary>
        /// Get a section from the configuration
        /// </summary>
        /// <typeparam name="T">the section's type</typeparam>
        /// <returns>the reference to that section</returns>
        public T Get<T>() where T : class, IConfig;
    }
}
