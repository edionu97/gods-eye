using GodsEye.Utility.Configuration.Base;

namespace GodsEye.Utility.Configuration.Sections.Camera
{
    public class ImageOptionsSectionConfig : IConfig
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
