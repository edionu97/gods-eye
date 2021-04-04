using GodsEye.Utility.Application.Config.BaseConfig.Abstract;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using GodsEye.Utility.Application.Config.Configuration.Sections.RabbitMq;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;

namespace GodsEye.Utility.Application.Config.Configuration.Impl
{
    public class AppConfig : AbstractConfig
    {
        public CameraSectionConfig Camera { get; set; }
        public RemoteWorkerSectionConfig RemoteWorker { get; set; }
        public RabbitMqConfig RabbitMessageQueueServer { get; set; }
    }
}
