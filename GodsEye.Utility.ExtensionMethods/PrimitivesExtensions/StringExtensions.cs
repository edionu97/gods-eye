using System;
using System.Text.RegularExpressions;

namespace GodsEye.Utility.ExtensionMethods.PrimitivesExtensions
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
            catch(Exception)
            {
                return null;
            }
        }
    }
}
