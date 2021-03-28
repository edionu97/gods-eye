using System.IO;
using System.Collections.Generic;

namespace GodsEye.ImageStreaming.ImageSource.ImageProvider
{
    public interface IImageLocator
    {
        public IEnumerable<FileInfo> LocateImages(string locationId);
    }
}
