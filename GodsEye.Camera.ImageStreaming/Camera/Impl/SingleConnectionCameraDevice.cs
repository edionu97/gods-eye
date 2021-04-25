using System;
using System.Diagnostics;
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
            var logger = _loggerFactory.CreateLogger($"{deviceId}itemsToKeep");

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

                    //create the frame synchronizer
                    var frameStopWatch = new Stopwatch();

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

                        //start the stopwatch
                        frameStopWatch.Start();

                        //iterate through the available images
                        var timeToRecover = .0;
                        await foreach (var frameInfo in _imageProvider.ProvideImages(deviceId))
                        {
                            //time before sending
                            var beforeSendingTime = frameStopWatch.Elapsed;

                            //send frame to the client
                            await SendFrameToClientAsync(client, frameInfo, imageType);

                            //time after sending
                            var sendingTime = (frameStopWatch.Elapsed - beforeSendingTime).TotalMilliseconds;

                            //wait the interval in order to match the desired fps
                            timeToRecover = await SyncWithTheDesiredFrameRateAsync(fps, sendingTime, timeToRecover);
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
                    finally
                    {
                        //stop the synchronizer
                        frameStopWatch.Stop();
                    }
                }

                // ReSharper disable once FunctionNeverReturns
            });
        }
    }
}
