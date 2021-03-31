namespace GodsEye.Utility.Application.Config.Settings.RemoteWorker
{
    public interface IWebSocketSettings
    {
        public int ServerStartingPort { get; set; }

        public string ServerAddress { get; set; }

        public void Deconstruct(out int port, out string address)
        {
            port = ServerStartingPort;
            address = ServerAddress;
        }
    }
}
