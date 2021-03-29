﻿using System.Threading.Tasks;

namespace GodsEye.ImageStreaming.Camera.Camera
{
    public interface ICameraDevice
    {
        public Task StartSendingImageFrames(string deviceId, int devicePort);
    }
}
