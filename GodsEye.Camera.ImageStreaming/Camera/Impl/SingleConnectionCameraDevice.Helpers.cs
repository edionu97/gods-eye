using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using GodsEye.Camera.ImageStreaming.Messages;
using GodsEye.Utility.Application.Config.Settings;
using GodsEye.Utility.Application.Helpers.Helpers.Network;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Application.Config.Settings.Camera;
using GodsEye.Utility.Application.Security.Encryption;
using Microsoft.Extensions.Logging;

namespace GodsEye.Camera.ImageStreaming.Camera.Impl
{
    public partial class SingleConnectionCameraDevice
    {
        private readonly IImageProvider _imageProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ICameraSettings _cameraSettings;
        private readonly IEncryptorDecryptor _encryptor;

        private async Task WaitFrameIntervalAsync()
        {
            //get the frame interval
            var frameInterval =
                Math.Ceiling(1000.0 / _cameraSettings.FramesPerSecond);

            //make the delay
            await Task.Delay(TimeSpan.FromMilliseconds(frameInterval));
        }

        private async Task SendFrameToClientAsync(Socket client, Tuple<string, byte[]> imageFrame)
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

            //send the message to the client and encrypt the message
            await SendHelpers
                .SendMessageAsync<ImageFrameMessage>(imageFrameMessage, client, _encryptor);
        }
    }
}
