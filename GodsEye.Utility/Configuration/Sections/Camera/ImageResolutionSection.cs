namespace GodsEye.Utility.Configuration.Sections.Camera
{
    public class ImageResolutionSection
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
