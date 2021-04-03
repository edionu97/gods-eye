﻿namespace GodsEye.Utility.Application.Items.Messages.Registration
{
    public class OnlineCameraMessage
    {
        public string CameraIp { get; set; }

        public int CameraPort { get; set; }

        public void Deconstruct(out string cameraIp, out int cameraPort)
        {
            cameraPort = CameraPort;
            cameraIp = CameraIp;
        }
    }
}
