using GodsEye.Utility.Application.Config.BaseConfig.Abstract;
using GodsEye.Utility.Application.Config.Configuration.Sections.RabbitMq;

namespace GodsEye.Application.Api.Config
{
    public class ApplicationConfig : AbstractConfig
    {
        public RabbitMqConfig RabbitMq { get; set; }
        public WsServerConfig WsServer { get; set; }
    }
}
