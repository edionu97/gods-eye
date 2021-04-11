using System.IO;
using GodsEye.Utility.Application.Items.Exceptions;
using LocalConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.PathHelpers;

namespace GodsEye.Utility.Application.Helpers.Helpers.Paths
{
    public static class PathHelpers
    {
        /// <summary>
        /// Resolve a path (relative path) in absolute path
        /// </summary>
        /// <param name="path">the string that represents the path</param>
        /// <returns>the full path</returns>
        /// <exception cref="PathEmptyOrWhitespaceException">if the path is null or empty</exception>
        public static string ResolvePath(string path)
        {
            //treat the case in which the path is empty
            if (string.IsNullOrEmpty(path))
            {
                throw
                    new PathEmptyOrWhitespaceException(
                        string.Format(
                            LocalConstants.PathNullOrEmptyMessage, path));
            }

            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Create  the path to the file (all the directories on the way)
        /// </summary>
        /// <param name="path">the path</param>
        /// <returns>a file info to the path</returns>
        /// <exception cref="PathEmptyOrWhitespaceException">if the path is null or empty</exception>
        public static FileInfo CreatePathToFile(string path)
        {
            //check the null values
            if (string.IsNullOrEmpty(path))
            {
                throw new PathEmptyOrWhitespaceException(path);
            }

            //get the parent directory path
            var directoryPath = path
                .Replace(Path.GetFileName(path), "");

            //create the directory
            Directory.CreateDirectory(directoryPath);

            return new FileInfo(path);
        }
    }
}
