using System;
using System.IO;
using System.Text.RegularExpressions;
using GodsEye.Utility.Application.Items.Exceptions;
using GodsEye.Utility.Application.Items.Constants.Message;

namespace GodsEye.Utility.Application.Helpers.ExtensionMethods.PrimitivesExtensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a string into an regex
        /// </summary>
        /// <param name="string">the string that will be converted</param>
        /// <returns>null if the regex is not valid or an regex otherwise</returns>
        public static Regex ToRegex(this string @string)
        {
            try
            {
                return new Regex(@string);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Convert a string in either a directory info either a file info
        /// </summary>
        /// <typeparam name="T">either directory info either file info</typeparam>
        /// <param name="path">the value of the path</param>
        /// <returns>an instance of the file system info</returns>
        public static T AsFileSystemInfo<T>(this string path) where T : FileSystemInfo
        {
            //treat the empty or null path
            if (string.IsNullOrEmpty(path))
            {
                throw new PathEmptyOrWhitespaceException(
                    string.Format(
                        MessageConstants.PathHelpers.PathNullOrEmptyMessage,
                        path));
            }
            
            //create either a directory either a file info
            return Activator.CreateInstance(typeof(T), path) as T;
        }
    }
}
