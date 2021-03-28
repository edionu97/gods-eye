using System.IO;
using System.Collections.Generic;

namespace GodsEye.ImageStreaming.ImageSource.ImageProvider
{
    public interface IImageProvider
    {
        public IEnumerable<FileInfo> ProvideImagesFromSource(string locationId);
    }
}
