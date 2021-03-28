using System.Collections.Generic;
using System.IO;

namespace GodsEye.ImageStreaming.ImageSource.ImageLocator
{
    public interface IImageLocator
    {
        public IEnumerable<FileInfo> LocateImages(string locationId);
    }
}
