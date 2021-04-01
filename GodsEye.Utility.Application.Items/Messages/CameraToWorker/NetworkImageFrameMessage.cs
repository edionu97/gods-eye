using System;
using GodsEye.Utility.Application.Items.Enums;

namespace GodsEye.Utility.Application.Items.Messages.CameraToWorker
{
    [Serializable]
    public class NetworkImageFrameMessage : ISerializableByteNetworkMessage
    {
        public ImageType ImageType { get; set; }
        public string FrameName { get; set; }
        public string ImageBase64EncodedBytes { get; set; }
    }
}
