using GodsEye.Utility.Configuration.Sections.Camera;
using GodsEye.Utility.Configuration.Sections.Resources;

namespace GodsEye.Utility.Configuration.Impl
{
    public class ApplicationSettings : IApplicationSettings
    {
        public CameraSection Camera { get; set; }

        public ResourcesSection Resources { get; set; }

        public void Deconstruct(
            out CameraSection camera, out ResourcesSection resources)
        {
            camera = Camera;
            resources = Resources;
        }
    }
}
