using System;

namespace GodsEye.Utility.Exceptions
{
    public class PathEmptyOrWhitespaceException : Exception
    {
        public PathEmptyOrWhitespaceException(string message)
            : base(message)
        {

        }
    }
}
