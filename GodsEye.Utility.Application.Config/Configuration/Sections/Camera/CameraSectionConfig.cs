using GodsEye.Utility.Application.Helpers.Helpers.Paths;
using GodsEye.Utility.Application.Config.Configuration.Base;

namespace GodsEye.Utility.Application.Config.Configuration.Sections.Camera
{
    public class CameraSectionConfig : IConfig
    {
        private string _imageDirectoryPath;
        public string ImageDirectoryPath
        {
            set => _imageDirectoryPath = value;
            get
            {
                //resolve the path if possible
                if (!string.IsNullOrEmpty((_imageDirectoryPath)))
                {
                    _imageDirectoryPath =
                        PathHelpers.ResolvePath(_imageDirectoryPath);
                }

                return _imageDirectoryPath;
            }
        }

        public string CameraId { get; set; }
        public NetworkSectionConfig Network { get; set; }
        public ImageOptionsSectionConfig ImageOptions { get; set; }
    }
}
