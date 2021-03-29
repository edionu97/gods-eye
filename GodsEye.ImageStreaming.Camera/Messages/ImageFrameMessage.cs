using System;
using GodsEye.Utility.Enums;
using GodsEye.Utility.Helpers.Network.Message;

namespace GodsEye.ImageStreaming.Camera.Messages
{
    [Serializable]
    public class ImageFrameMessage : INetworkMessage
    {
        public string ImageBase64EncodedBytes { get; set; }
        public string FrameName { get; set; }
        public ImageType ImageType { get; set; }
    }
}
