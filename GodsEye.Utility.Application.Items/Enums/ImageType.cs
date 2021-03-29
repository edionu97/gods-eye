using System;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace GodsEye.Utility.Application.Items.Enums
{
    public enum ImageType
    {
        Png,
        Bmp,
        Jpeg
    }

    public static class ImageTypeExtensions
    {
        /// <summary>
        /// This function it is used for getting the proper formatter based on image type
        /// </summary>
        /// <param name="imageType">type of the image</param>
        /// <returns>PngFormat || BmpFormat || JpegFormat</returns>
        public static IImageFormat ToFormat(this ImageType imageType)
        {
            return imageType switch
            {
                ImageType.Png => PngFormat.Instance,
                ImageType.Bmp => BmpFormat.Instance,
                ImageType.Jpeg => JpegFormat.Instance,
                _ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null)
            };
        }
    }
}
