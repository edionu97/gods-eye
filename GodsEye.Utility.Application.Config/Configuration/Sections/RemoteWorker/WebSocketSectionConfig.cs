using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class WebSocketSectionConfig : AbstractConfig
    {
        public int ServerStartingPort { get; set; }

        public string ServerAddress { get; set; }

        public void Deconstruct(out int port, out string address)
        {
            address = ServerAddress;
            port = ServerStartingPort;
        }
    }
}
