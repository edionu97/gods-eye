using System;

namespace GodsEye.Utility.Exceptions.CustomExceptions
{
    public class PathEmptyOrWhitespaceException : Exception
    {
        public PathEmptyOrWhitespaceException(string message)
            : base(message)
        {

        }
    }
}
