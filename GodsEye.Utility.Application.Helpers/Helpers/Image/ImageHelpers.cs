using System;
using System.Linq;
using ImageMagick;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using GodsEye.Utility.Application.Items.Enums;

using ImageSharp = SixLabors.ImageSharp.Image;

namespace GodsEye.Utility.Application.Helpers.Helpers.Image
{
    public static class ImageHelpers
    {
        /// <summary>
        /// This function it is used for resizing an image that is in base 64 format to a specific width and height
        /// </summary>
        /// <param name="base64String">the string representing the image</param>
        /// <param name="resizeTo">the tuple which contains info about the with and height</param>
        /// <returns>the base64 image</returns>
        public static string ResizeBase64Image(string base64String, (int Width, int Height) resizeTo)
        {
            //if the image is null then return null
            var img = ConvertImageFromBase64(base64String);
            if (img == null)
            {
                return null;
            }

            //spliced the 
            var splicedString = base64String
                .Split(',').FirstOrDefault();

            //load the image in memory
            var loadedImage = ImageSharp.Load(img);

            //do the resize nothing
            var (w, h) = resizeTo;

            //resize the image to desired resolution
            loadedImage.Mutate(context =>
                context.Resize(w, h, KnownResamplers.Lanczos8));

            //parse the enum
            Enum.TryParse<ImageType>(splicedString, true, out var type);

            //convert the image back in the base64
            return loadedImage.ToBase64String(type.ToFormat());
        }

        /// <summary>
        /// Compare the image instances
        /// </summary>
        /// <param name="firstImage">the first image</param>
        /// <param name="secondImage">the second image</param>
        /// <returns></returns>
        public static (ImageComparisonResultType, double) CompareImages(MagickImage firstImage, MagickImage secondImage)
        {
            //compare those two images using the perceptual hash
            var comparisonResult = firstImage.Compare(secondImage, ErrorMetric.PerceptualHash);

            //return the comparisonResult 
            return comparisonResult switch
            {
                var x when x < 10 => (ImageComparisonResultType.Same, x),
                var x when x < 20 => (ImageComparisonResultType.Similar, x),
                var x when x < 25 => (ImageComparisonResultType.RoughlySimilar, x),
                var x => (ImageComparisonResultType.Different, x)
            };
        }

        /// <summary>
        /// Convert a base64 image representation into a string
        /// </summary>
        /// <param name="base64String">the base64String image</param>
        /// <returns>the converted byte array</returns>
        public static byte[] ConvertImageFromBase64(string base64String)
        {
            //treat the null case
            if (string.IsNullOrEmpty(base64String))
            {
                return null;
            }

            //spliced the 
            var splicedString = base64String
                .Split(',').Last();

            //return the image
            return Convert.FromBase64String(splicedString);
        }
    }
}
