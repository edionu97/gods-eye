using System.IO;
using System.Collections.Generic;
using GodsEye.Utility.Application.Config.Settings;
using GodsEye.Utility.Application.Helpers.ExtensionMethods.PrimitivesExtensions;

namespace GodsEye.Camera.ImageStreaming.ImageSource.ImageLocator.Impl
{
    public class LocalFileSystemJpegImageLocator : IImageLocator
    {
        private readonly ICameraSettings _cameraSettings;

        public LocalFileSystemJpegImageLocator(ICameraSettings cameraSettings)
        {
            _cameraSettings = cameraSettings;
        }

        public IEnumerable<FileInfo> LocateImages(string locationId)
        {
            //get the directory path
            var directoryPath = Path
                .Combine(_cameraSettings.ImageDirectoryPath, locationId)
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
