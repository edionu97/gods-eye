using System;
using System.Collections.Generic;

namespace GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider
{
    public interface IImageProvider
    {
        /// <summary>
        /// Gets all images from a location
        /// </summary>
        /// <param name="locationId">location's identifier</param>
        /// <returns>a tuple of elements representing the association between the name of the file and the file bytes</returns>
        public IAsyncEnumerable<Tuple<string, string>> ProvideImages(string locationId);
    }
}
