using System.IO;
using GodsEye.Utility.Exceptions.CustomExceptions;

using LocalConstants = GodsEye.Utility.Constants.Hardcoded.Message.MessageConstants.PathHelpers;

namespace GodsEye.Utility.Helpers.Files.Paths
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
    }
}
