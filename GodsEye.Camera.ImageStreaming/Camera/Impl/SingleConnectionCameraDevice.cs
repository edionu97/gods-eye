using System;
using EasyNetQ;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using LocalConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.CameraDevice;

namespace GodsEye.Camera.ImageStreaming.Camera.Impl
{
    public partial class SingleConnectionCameraDevice : ICameraDevice
    {
        private readonly IBus _registrationQueue;

        public SingleConnectionCameraDevice(
            IImageProvider imageProvider,
            ILoggerFactory loggerFactory,
            IEncryptorDecryptor encryptor,
            IConfig configuration,
            IBus registrationMessageQueue)
        {
            //set the config
            _applicationConfig = configuration;

            _imageProvider = imageProvider;
            _encryptor = encryptor;
            _loggerFactory = loggerFactory;

            _registrationQueue = registrationMessageQueue;
        }

        public Task StartSendingImageFrames(string deviceId)
        {
            //get the network config
            var (imageType, cameraAddress) = _applicationConfig
                .Get<NetworkSectionConfig>();

            //get the image resolution section config
            var (fps, (width, height)) =
                _applicationConfig.Get<ImageOptionsSectionConfig>();

            //create the logger
            var logger = _loggerFactory.CreateLogger($"{deviceId}_");

            //log the message
            logger
                .LogInformation(LocalConstants.CameraIsInitializedMessage);

            //create a new task on thread pool
            return Task.Run(async () =>
            {
                //if the existing connection fails => accept other client
                while (true)
                {
                    //start the tcp listener
                    var (clientListener, streamingPort) = StartTcpListener(cameraAddress);

                    //display the message
                    logger.LogInformation(
                        LocalConstants.CameraIsStreamingMessage, streamingPort);

                    //send the images frame by frame to client
                    try
                    {
                        //register the camera
                        RegisterThisCamera(cameraAddress, streamingPort);

                        //wait for a single connection
                        using var client = await clientListener.AcceptSocketAsync();

                        //log the camera data
                        LogCameraData(
                            cameraAddress, 
                            streamingPort, 
                            width, height, fps, imageType, logger);

                        //iterate through the available images
                        await foreach (var frameInfo in _imageProvider.ProvideImages(deviceId))
                        {
                            //send frame to the client
                            await SendFrameToClientAsync(client, frameInfo, imageType);

                            //wait the interval in order to match the desired fps
                            await WaitFrameIntervalAsync(fps);
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
