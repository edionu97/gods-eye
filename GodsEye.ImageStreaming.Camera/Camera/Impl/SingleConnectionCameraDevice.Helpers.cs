using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using GodsEye.Utility.Configuration;
using GodsEye.Utility.Helpers.Network;
using GodsEye.ImageStreaming.Camera.Messages;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Settings;

namespace GodsEye.ImageStreaming.Camera.Camera.Impl
{
    public partial class SingleConnectionCameraDevice
    {
        private readonly IImageProvider _imageProvider;
        private readonly ICameraSettings _cameraSettings;

        private async Task WaitFrameInterval()
        {
            //get the frame interval
            var frameInterval =
                Math.Ceiling(1000.0 / _cameraSettings.FramesPerSecond);

            //make the delay
            await Task.Delay(TimeSpan.FromMilliseconds(frameInterval));
        }

        private void SendFrameToClient(Socket client, Tuple<string, byte[]> imageFrame)
        {
            //deconstruct the object
            var (frameName, frameBytes) = imageFrame;

            //create the message that will be framed
            var imageFrameMessage = new ImageFrameMessage
            {
                FrameName = frameName,
                ImageBase64EncodedBytes = Convert.ToBase64String(frameBytes),
                ImageType = _cameraSettings.StreamingImageType
            };

            //send the message to the client
            SendHelpers.SendMessage<ImageFrameMessage>(imageFrameMessage, client);
        }
    }
}
