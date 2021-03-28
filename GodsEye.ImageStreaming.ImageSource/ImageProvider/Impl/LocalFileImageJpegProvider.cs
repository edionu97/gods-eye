using System.IO;
using System.Collections.Generic;
using GodsEye.Utility.Configuration;
using GodsEye.Utility.ExtensionMethods.PrimitivesExtensions;

namespace GodsEye.ImageStreaming.ImageSource.ImageProvider.Impl
{
    public class LocalFileImageJpegProvider : IImageProvider
    {
        private readonly IApplicationSettings _appSettings;

        public LocalFileImageJpegProvider(IApplicationSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public IEnumerable<FileInfo> ProvideImagesFromSource(string locationId)
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
