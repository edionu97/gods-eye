using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageLocator;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using GodsEye.Utility.Application.Items.Enums;

namespace GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider.Impl
{
    public class ContinuouslyImageProvider : IImageProvider
    {
        private readonly ImageType _streamImageType;
        private readonly IImageLocator _imageLocator;
        private readonly ImageOptionsSectionConfig _imageConfig;

        public ContinuouslyImageProvider(
            IImageLocator imageLocator, IConfig appConfig)
        {
            _imageLocator = imageLocator;

            //set the image config
            _imageConfig = appConfig.Get<ImageOptionsSectionConfig>();
            _streamImageType = appConfig.Get<NetworkSectionConfig>().ImageStreamingFormat;
        }

        public async IAsyncEnumerable<Tuple<string, string>> ProvideImages(string locationId)
        {
            //create a dictionary for items
            var byteImageDictionary = new Dictionary<string, string>();
            await foreach (var (imagesBase64, imageName) in GetImages(locationId))
            {
                //return the images
                yield return Tuple
                    .Create(imageName, imagesBase64);

                //cache images
                byteImageDictionary.Add(imageName, imagesBase64);
            }

            //stream the images
            var fileNames = new Queue<string>(byteImageDictionary.Keys);
            while (fileNames.Any())
            {
                //get the file info
                var fileName = fileNames.Dequeue();

                //return the image bytes
                yield return Tuple
                    .Create(fileName, byteImageDictionary[fileName]);

                //enqueue the file info back
                //so it will create a never ending cycle
                fileNames.Enqueue(fileName);
            }
        }

        /// <summary>
        /// Gets all the images from the specified location
        /// </summary>
        /// <param name="imageLocation">the location of the image folder (relative to the path specified in json)</param>
        /// <returns>a tuple of items that represent the image bytes and the name of the file image</returns>
        private async IAsyncEnumerable<Tuple<string, string>> GetImages(string imageLocation)
        {
            //destruct the image config
            var (_, (width, height)) = _imageConfig;

            //iterate through all the image files from the location
            foreach (var imageFile in _imageLocator.LocateImages(imageLocation))
            {
                //load the image in memory
                var loadedImage =
                    await Image.LoadAsync<Rgba32>(imageFile.FullName);

                //resize the image to desired resolution
                loadedImage.Mutate(context =>
                    context.Resize(width, height, KnownResamplers.Lanczos8));

                //create a tuple of elements
                var newFileName =
                    $"{Path.GetFileNameWithoutExtension(imageFile.Name)}.{_streamImageType.ToString().ToLower()}";
                yield return Tuple.Create(loadedImage.ToBase64String(_streamImageType.ToFormat()), newFileName);
            }
        }

    }
}
