using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Config.Settings.Camera;
using GodsEye.Utility.Application.Helpers.Helpers.Network;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

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

        private async Task SendFrameToClientAsync(Socket client, Tuple<string, string> imageFrame)
        {
            //deconstruct the object
            var (frameName, imageBase64EncodedBytes) = imageFrame;

            //create the message that will be framed
            var imageFrameMessage = new NetworkImageFrameMessage
            {
                FrameName = frameName,
                ImageType = _cameraSettings.StreamingImageType,
                ImageBase64EncodedBytes = imageBase64EncodedBytes
            };

            //send the message to the client and encrypt the message
            await SendHelpers
                .SendMessageAsync<NetworkImageFrameMessage>(imageFrameMessage, client, _encryptor);
        }
    }
}
