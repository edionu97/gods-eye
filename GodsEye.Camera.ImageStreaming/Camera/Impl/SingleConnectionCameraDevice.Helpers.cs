using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Helpers.Helpers.Network;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
using GodsEye.Utility.Application.Items.Messages.Registration;

using LocalConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.CameraDevice;


namespace GodsEye.Camera.ImageStreaming.Camera.Impl
{
    public partial class SingleConnectionCameraDevice
    {
        private readonly IImageProvider _imageProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IEncryptorDecryptor _encryptor;
        private readonly IConfig _applicationConfig;


        private static void LogCameraData(
            string cameraAddress,
            int streamingPort,
            int width,
            int height,
            int fps,
            ImageType imageType,
            ILogger logger)
        {
            //message to write on console
            var message = JsonSerializerDeserializer<dynamic>.Serialize(
                new
                {
                    CameraAdress = $"{cameraAddress}:{streamingPort}",
                    Resolution = $"{width}x{height}",
                    Fps = fps,
                    ImageFormat = imageType.ToString()
                });

            //write the camera data
            logger.LogInformation(
                LocalConstants.CameraIsStreamingImagesMessage, message);
        }

        private void RegisterThisCamera(string cameraAddress, int cameraPort)
        {
            _registrationQueue?.PubSub.PublishAsync(new OnlineCameraMessage
            {
                CameraIp = cameraAddress,
                CameraPort = cameraPort
            });
        }

        private Tuple<TcpListener, int> StartTcpListener(string cameraAddress)
        {
            //get the streaming port
            var streamingPort = PortAllocationHelpers.GetNextTcpAvailablePort();

            //parse the address 
            var ipAddress = IPAddress.Parse(cameraAddress);

            //get the listener
            var clientListener = new TcpListener(ipAddress, streamingPort);

            //start the client listener
            clientListener.Start();

            //return the listener
            return Tuple.Create(clientListener, streamingPort);
        }


        private static async Task WaitFrameIntervalAsync(double desiredFps)
        {
            //get the frame interval
            var frameInterval =
                Math.Ceiling(1000.0 / desiredFps);

            //make the delay
            await Task.Delay(TimeSpan.FromMilliseconds(frameInterval));
        }

        private async Task SendFrameToClientAsync(
            Socket client, Tuple<string, string> imageFrame, ImageType imageType)
        {
            //deconstruct the object
            var (frameName, imageBase64EncodedBytes) = imageFrame;

            //create the message that will be framed
            var imageFrameMessage = new NetworkImageFrameMessage
            {
                FrameName = frameName,
                ImageType = imageType,
                ImageBase64EncodedBytes = imageBase64EncodedBytes
            };

            //send the message to the client and encrypt the message
            await SendHelpers
                .SendMessageAsync<NetworkImageFrameMessage>(imageFrameMessage, client, _encryptor);
        }
    }
}
