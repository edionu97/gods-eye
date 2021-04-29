using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using GodsEye.Utility.Application.Items.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using ImageSharp = SixLabors.ImageSharp.Image;

namespace GodsEye.Utility.Application.Helpers.Helpers.Image
{
    public static class ImageHelpers
    {
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
        /// Convert the image from a file into an base64 string
        /// </summary>
        /// <param name="imagePath">the path of the image</param>
        /// <param name="imageType">the image type</param>
        /// <param name="resizeTo">if specified contains the width and height</param>
        /// <returns>the conversion in base64 of the image</returns>
        public static async Task<string> ReadImageFromBase64FileAsync(
            FileInfo imagePath, 
            ImageType imageType, 
            (int Width, int Height)? resizeTo = null)
        {
            //if the image does not exist then return null
            if (!imagePath.Exists)
            {
                return null;
            }

            //load the image in memory
            var loadedImage = 
                await ImageSharp.LoadAsync<Rgba32>(imagePath.FullName);

            //if the resize is not specified return the image as it is
            if (!resizeTo.HasValue)
            {
                return loadedImage.ToBase64String(imageType.ToFormat());
            }

            //do the resize nothing
            var (w, h) = resizeTo.Value;
            
            //resize the image to desired resolution
            loadedImage.Mutate(context =>
                context.Resize(w, h, KnownResamplers.Lanczos8));

            //convert the image in base64 string
            return loadedImage.ToBase64String(imageType.ToFormat());
        }


        /// <summary>
        /// Compare two images using the PHash algorithm
        /// </summary>
        /// <param name="firstImageBase64">the first image</param>
        /// <param name="secondImageBase64">the second image</param>
        /// <returns>an enum with 4 values
        ///     ImageComparisonResultType.Same if the distance is less than 10
        ///     ImageComparisonResultType.Similar if the distance is between 10 and 20
        ///     ImageComparisonResultType.RoughlySimilar if the distance is between 20 and 25
        ///     ImageComparisonResultType.Different otherwise
        /// </returns>
        public static (ImageComparisonResultType, double) CompareB64Images(string firstImageBase64, string secondImageBase64)
        {
            //return different
            if (string.IsNullOrEmpty(firstImageBase64) || string.IsNullOrEmpty(secondImageBase64))
            {
                return (ImageComparisonResultType.Different, double.MaxValue);
            }

            //load the first image
            var firstImage = new MagickImage(ConvertImageFromBase64(firstImageBase64));
            var secondImage = new MagickImage(ConvertImageFromBase64(secondImageBase64));

            //compare the images
            return CompareImages(firstImage, secondImage);
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
