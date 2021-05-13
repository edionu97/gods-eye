using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Application.Api.Config
{
    public class WsServerConfig : AbstractConfig
    {
        public string Address { get; set; }

        public int Port { get; set; }

        public void Deconstruct(out string address, out int port)
        {
            address = Address;
            port = Port;
        }
    }
}
