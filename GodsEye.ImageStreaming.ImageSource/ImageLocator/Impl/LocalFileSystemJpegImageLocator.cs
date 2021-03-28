using System.Collections.Generic;
using System.IO;
using GodsEye.Utility.Configuration;
using GodsEye.Utility.ExtensionMethods.PrimitivesExtensions;

namespace GodsEye.ImageStreaming.ImageSource.ImageLocator.Impl
{
    public class LocalFileSystemJpegImageLocator : IImageLocator
    {
        private readonly IApplicationSettings _appSettings;

        public LocalFileSystemJpegImageLocator(IApplicationSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public IEnumerable<FileInfo> LocateImages(string locationId)
        {
            //get the directory path
            var directoryPath = Path
                .Combine(_appSettings.Resources.ImageDirectoryPath, locationId)
                .AsFileSystemInfo<DirectoryInfo>();

            //break if the directory does not exist
            if (!directoryPath.Exists)
            {
                yield break;
            }

            //iterate each jpeg file
            foreach (var jpegFile in directoryPath.GetFiles("*.jpg"))
            {
                yield return jpegFile;
            }
        }
    }
}
