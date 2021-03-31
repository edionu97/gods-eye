using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.Utility.Application.Config.Settings;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Application.Config.Settings.Camera;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Security.KeyProvider;
using LocalConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.CameraDevice;

namespace GodsEye.Camera.ImageStreaming.Camera.Impl
{
    public partial class SingleConnectionCameraDevice : ICameraDevice
    {
        private readonly string _encryptionKey;

        public SingleConnectionCameraDevice(
            IImageProvider imageProvider, 
            ICameraSettings cameraSettings, 
            ILoggerFactory loggerFactory,
            IKeyProvider provider,
            IEncryptorDecryptor encryptor)
        {
            _encryptor = encryptor;
            _loggerFactory = loggerFactory;
            _imageProvider = imageProvider;
            _cameraSettings = cameraSettings;

            //get the encryption key
            _encryptionKey = provider?.GetKey();
        }

        public Task StartSendingImageFrames(string deviceId, int devicePort)
        {
            //create the logger
            var logger = _loggerFactory.CreateLogger($"{deviceId}_");

            //log the message
            logger
                .LogInformation(LocalConstants.CameraIsInitializedMessage);

            //create a new task on thread pool
            return Task.Run(async () =>
            {
                //parse the address 
                var ipAddress = IPAddress.Parse(_cameraSettings.CameraAddress);

                //get the listener
                var clientListener = new TcpListener(ipAddress, devicePort);

                //start the client listener
                clientListener.Start();

                //if the existing connection fails => accept other client
                while (true)
                {
                    //display the message
                    logger.LogInformation(
                        LocalConstants.CameraIsStreamingMessage, devicePort);

                    //send the images frame by frame to client
                    try
                    {
                        //wait for a single connection
                        using var client = await clientListener.AcceptSocketAsync();

                        //message to write on console
                        var message = JsonSerializerDeserializer<dynamic>.Serialize(
                            new
                            {
                                CameraAdress = _cameraSettings.CameraAddress,
                                Resolution = $"{_cameraSettings.StreamingWidth}x{_cameraSettings.StreamingHeight}",
                                Fps = _cameraSettings.FramesPerSecond,
                                ImageFormat = _cameraSettings.StreamingImageType.ToString()
                            });

                        //write the camera data
                        logger.LogInformation(
                            LocalConstants.CameraIsStreamingImagesMessage, message);

                        //iterate through the available images
                        await foreach (var frameInfo in _imageProvider.ProvideImages(deviceId))
                        {
                            //send frame
                            await SendFrameToClientAsync(client, frameInfo);

                            //wait the interval in order to match the desired fps
                            await WaitFrameIntervalAsync();
                        }
                    }
                    catch (SocketException)
                    {
                        //treat the client disconnected message
                        logger.LogWarning(LocalConstants.StreamingLocationLostMessage);
                    }
                    catch (Exception e)
                    {
                        //treat other exceptions
                        logger.LogError(e, e.Message);
                    }
                }

                // ReSharper disable once FunctionNeverReturns
            });
        }
    }
}
