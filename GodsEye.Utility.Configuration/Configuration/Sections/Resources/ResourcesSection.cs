using GodsEye.Utility.Helpers.Files.Paths;

namespace GodsEye.Utility.Configuration.Configuration.Sections.Resources
{
    public class ResourcesSection
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
    }
}
