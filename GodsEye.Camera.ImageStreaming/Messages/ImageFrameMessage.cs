using System;
using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Helpers.Helpers.Network.Message;

namespace GodsEye.Camera.ImageStreaming.Messages
{
    [Serializable]
    public class ImageFrameMessage : INetworkMessage
    {
        public string ImageBase64EncodedBytes { get; set; }
        public string FrameName { get; set; }
        public ImageType ImageType { get; set; }
    }
}
