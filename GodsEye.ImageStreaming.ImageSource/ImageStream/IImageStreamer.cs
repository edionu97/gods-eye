using System.Collections.Generic;

namespace GodsEye.ImageStreaming.ImageSource.ImageStream
{
    public interface IImageStreamer
    {
        public IAsyncEnumerable<(string ImageName, byte[] ImageBytes)> StreamImages(string locationId);
    }
}
