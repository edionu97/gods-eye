using GodsEye.Utility.Application.Config.Configuration.Base;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;

namespace GodsEye.Utility.Application.Config.Configuration
{
    public interface IAppConfig : IConfig
    {
        public CameraSectionConfig Camera { get; set; }

        public RemoteWorkerSectionConfig RemoteWorker { get; set; }
    }
}
