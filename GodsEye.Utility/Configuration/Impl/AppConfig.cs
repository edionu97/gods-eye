using GodsEye.Utility.Configuration.Sections.Camera;

namespace GodsEye.Utility.Configuration.Impl
{
    public class AppConfig : IAppConfig
    {
        public CameraSectionConfig Camera { get; set; }
    }
}
