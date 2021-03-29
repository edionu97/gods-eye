using System;

namespace GodsEye.Utility.Exceptions
{
    public class CameraDisconnectedException : Exception
    {
        public CameraDisconnectedException(string message)
            : base(message)
        {
        }
    }
}
