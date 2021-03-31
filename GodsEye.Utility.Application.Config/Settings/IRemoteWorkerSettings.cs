namespace GodsEye.Utility.Application.Config.Settings
{
    public interface IRemoteWorkerSettings
    {
        public int ServerStartingPort { get; set; }

        public string ServerAddress { get; set; }
    }
}
