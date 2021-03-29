using GodsEye.Utility.Configuration.Base;

namespace GodsEye.Utility.Configuration.Sections.Camera
{
    public class ImageResolutionSectionConfig : IConfig
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
