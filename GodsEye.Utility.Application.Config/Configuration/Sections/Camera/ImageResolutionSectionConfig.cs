using GodsEye.Utility.Application.Config.BaseConfig.Abstract;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.Camera
{
    public class ImageResolutionSectionConfig : AbstractConfig
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public void Deconstruct(
            out int width, out int height)
        {
            width = Width;
            height = Height;
        }
    }
}
