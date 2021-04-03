using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class WorkerStartupMessageBrokerConfig : AbstractConfig
    {
        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public void Deconstruct(
            out int port, out string username, out string password)
        {
            port = Port;
            username = Username;
            password = Password;
        }
    }
}
