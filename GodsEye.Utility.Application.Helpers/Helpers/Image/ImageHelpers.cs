using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GodsEye.Utility.Application.Items.Enums;
using ImageMagick;

namespace GodsEye.Utility.Application.Helpers.Helpers.Image
{
    public static class ImageHelpers
    {
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
        public static ImageComparisonResultType CompareImages(string firstImageBase64, string secondImageBase64)
        {
            //return different
            if (string.IsNullOrEmpty(firstImageBase64) || string.IsNullOrEmpty(secondImageBase64))
            {
                return ImageComparisonResultType.Different;
            }

            //load the first image
            var firstImage = new MagickImage(ConvertImageFromBase64(firstImageBase64));
            var secondImage = new MagickImage(ConvertImageFromBase64(secondImageBase64));

            //compare those two images using the perceptual hash
            var comparisonResult = firstImage.Compare(secondImage, ErrorMetric.PerceptualHash);

            //return the comparisonResult 
            return comparisonResult switch
            {
                var x when x < 10 => ImageComparisonResultType.Same,
                var x when x < 20 => ImageComparisonResultType.Similar,
                var x when x < 25 => ImageComparisonResultType.RoughlySimilar,
                _ => ImageComparisonResultType.Different
            };
        }
    }
}
