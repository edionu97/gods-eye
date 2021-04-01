using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.Camera
{
    public class ImageOptionsSectionConfig : AbstractConfig
    {
        public int FramesPerSecond { get; set; }

        public ImageResolutionSectionConfig ImageResolution { get; set; }

        public void Deconstruct(out int fps, out ImageResolutionSectionConfig resolution)
        {
            fps = FramesPerSecond;
            resolution = ImageResolution;
        }
    }
}
