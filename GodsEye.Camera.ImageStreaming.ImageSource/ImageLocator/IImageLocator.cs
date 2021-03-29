using System.Collections.Generic;
using System.IO;

namespace GodsEye.Camera.ImageStreaming.ImageSource.ImageLocator
{
    public interface IImageLocator
    {
        public IEnumerable<FileInfo> LocateImages(string locationId);
    }
}
