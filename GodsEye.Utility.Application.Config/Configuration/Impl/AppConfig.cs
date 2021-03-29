using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;

namespace GodsEye.Utility.Application.Config.Configuration.Impl
{
    public class AppConfig : IAppConfig
    {
        public CameraSectionConfig Camera { get; set; }
    }
}
