using GodsEye.Utility.Configuration.Configuration.Sections.Camera;
using GodsEye.Utility.Configuration.Configuration.Sections.Resources;

namespace GodsEye.Utility.Configuration.Configuration.Impl
{
    public class ApplicationSettings : IApplicationSettings
    {
        public CameraSection Camera { get; set; }

        public ResourcesSection Resources { get; set; }
    }
}
