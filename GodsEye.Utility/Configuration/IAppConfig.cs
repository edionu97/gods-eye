using GodsEye.Utility.Configuration.Base;
using GodsEye.Utility.Configuration.Sections.Camera;

namespace GodsEye.Utility.Configuration
{
    public interface IAppConfig : IConfig
    {
        public CameraSectionConfig Camera { get; set; }
    }
}
