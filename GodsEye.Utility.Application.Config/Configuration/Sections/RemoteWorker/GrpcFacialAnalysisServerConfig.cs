using GodsEye.Utility.Application.Config.BaseConfig.Abstract;
using GodsEye.Utility.Application.Helpers.Helpers.Paths;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker
{
    public class GrpcFacialAnalysisServerConfig : AbstractConfig
    {
        private string _serverAuthCertificateLocation;
        public string ServerAuthCertificateLocation
        {
            get => _serverAuthCertificateLocation;
            set
            {
                if (string.IsNullOrEmpty(_serverAuthCertificateLocation = value))
                {
                    return;
                }

                _serverAuthCertificateLocation = PathHelpers.ResolvePath(value);
            }
        }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public void Deconstruct(
            out string serverAuthCertificateLocation,
            out string serverAddress, 
            out int serverPort)
        {
            serverAddress = ServerAddress;
            serverPort = ServerPort;
            serverAuthCertificateLocation = _serverAuthCertificateLocation;
        }
    }
}
