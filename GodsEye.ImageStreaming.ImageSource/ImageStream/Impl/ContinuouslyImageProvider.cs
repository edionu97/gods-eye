using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using GodsEye.Utility.Enums;
using System.Collections.Generic;
using GodsEye.Utility.Configuration;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;

namespace GodsEye.ImageStreaming.ImageSource.ImageStream.Impl
{
    public class ContinuouslyImageProvider : IImageProvider
    {
        private readonly IImageLocator _imageLocator;
        private readonly IApplicationSettings _applicationSettings;

        public ContinuouslyImageProvider(
            IImageLocator imageLocator, IApplicationSettings applicationSettings)
        {
            _imageLocator = imageLocator;
            _applicationSettings = applicationSettings;
        }

        public async IAsyncEnumerable<Tuple<string, byte[]>> ProvideImages(string locationId)
        {
            //create a dictionary for items
            var byteImageDictionary = new Dictionary<string, byte[]>();
            await foreach (var (imageBytes, imageName) in GetImages(locationId))
            {
                byteImageDictionary.Add(imageName, imageBytes);
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
        private async IAsyncEnumerable<Tuple<byte[], string>> GetImages(string imageLocation)
        {
            //deconstruct the object in properties
            var ((width, height), useResizeImages, imageType) 
                = _applicationSettings.Camera.ImageOptions;
            
            //iterate through all the image files from the location
            foreach (var imageFile in _imageLocator.LocateImages(imageLocation))
            {
                //load the image in memory
                var loadedImage = 
                    await Image.LoadAsync<Rgba32>(imageFile.FullName);

                //get the proper encoder
                var imageEncoder = loadedImage
                    .GetConfiguration()
                    .ImageFormatsManager
                    .FindEncoder(imageType.ToFormat());

                //create the memory stream
                await using var memoryStream = new MemoryStream();

                //resize the image if needed
                if (useResizeImages)
                {
                    loadedImage.Mutate(context => 
                        context.Resize(width, height, KnownResamplers.Lanczos8));
                }

                //save the image in the memory using the encoder
                await loadedImage.SaveAsync(memoryStream, imageEncoder);

                //create a tuple of elements
                var newFileName = 
                    $"{Path.GetFileNameWithoutExtension(imageFile.Name)}.{imageType.ToString().ToLower()}";
                yield return Tuple.Create(memoryStream.ToArray(), newFileName);
            }
        } 

    }
}
