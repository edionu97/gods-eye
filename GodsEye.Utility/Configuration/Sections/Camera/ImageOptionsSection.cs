using ImageType = GodsEye.Utility.Enums.ImageType;

namespace GodsEye.Utility.Configuration.Sections.Camera
{
    public class ImageOptionsSection
    {
        public  ImageResizeSection ImageResize { get; set; }

        public  ImageType ImageStreamingFormat { get; set; }

        public bool ResizeImages { get; set; }

        public void Deconstruct(
            out ImageResizeSection imageResizeSection, 
            out bool resizeImages,
            out ImageType imageType)
        {
            resizeImages = ResizeImages;
            imageResizeSection = ImageResize;
            imageType = ImageStreamingFormat;
        }
    }
}
