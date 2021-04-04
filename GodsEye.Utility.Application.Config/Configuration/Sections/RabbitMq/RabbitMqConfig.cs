using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RabbitMq
{
    public class RabbitMqConfig : AbstractConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public ushort Port { get; set; }

        public void Deconstruct(
            out string username,
            out string password, 
            out string host, out ushort port)
        {
            username = Username;
            password = Password;
            host = Host;
            port = Port;
        }
    }
}
