using System;

namespace GodsEye.Utility.Application.Items.Exceptions
{
    public class CameraDisconnectedException : Exception
    {
        public CameraDisconnectedException(string message)
            : base(message)
        {
        }
    }
}
