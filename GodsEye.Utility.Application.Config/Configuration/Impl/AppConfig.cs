using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;

namespace GodsEye.Utility.Application.Config.Configuration.Impl
{
    public class AppConfig : IAppConfig
    {
        public CameraSectionConfig Camera { get; set; }

        public RemoteWorkerSectionConfig RemoteWorker { get; set; }
    }
}
