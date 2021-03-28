namespace GodsEye.Utility.Configuration.Sections.Camera
{
    public class ImageResizeSection
    {
        public int ToWidth { get; set; }

        public int ToHeight { get; set; }

        public void Deconstruct(
            out int width, out int height)
        {
            width = ToWidth;
            height = ToHeight;
        }
    }
}
