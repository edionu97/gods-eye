using System;

namespace GodsEye.Utility.Application.Items.Exceptions
{
    public class PathEmptyOrWhitespaceException : Exception
    {
        public PathEmptyOrWhitespaceException(string message)
            : base(message)
        {

        }
    }
}
