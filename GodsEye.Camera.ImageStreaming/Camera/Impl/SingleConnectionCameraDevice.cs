﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Messages.Registration;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Security.KeyProvider;
using LocalConstants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.CameraDevice;

namespace GodsEye.Camera.ImageStreaming.Camera.Impl
{
    public partial class SingleConnectionCameraDevice : ICameraDevice
    {
        private readonly IBus _registrationQueue;

        public SingleConnectionCameraDevice(
            IImageProvider imageProvider,
            ILoggerFactory loggerFactory,
            IKeyProvider provider,
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

        public Task StartSendingImageFrames(string deviceId, int devicePort)
        {
            //get the network config
            var (imageType, port, cameraAddress) = _applicationConfig
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
                //parse the address 
                var ipAddress = IPAddress.Parse(cameraAddress);

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
                        //register the camera
                        RegisterThisCamera(cameraAddress, port);

                        //wait for a single connection
                        using var client = await clientListener.AcceptSocketAsync();

                        //message to write on console
                        var message = JsonSerializerDeserializer<dynamic>.Serialize(
                            new
                            {
                                CameraAdress = $"{cameraAddress}:{port}",
                                Resolution = $"{width}x{height}",
                                Fps = fps,
                                ImageFormat = imageType.ToString()
                            });

                        //write the camera data
                        logger.LogInformation(
                            LocalConstants.CameraIsStreamingImagesMessage, message);

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

        private void RegisterThisCamera(string cameraAddress, int cameraPort)
        {
            _registrationQueue?.PubSub.PublishAsync(new OnlineCameraMessage
            {
                CameraIp = cameraAddress,
                CameraPort = cameraPort
            });
        }
    }
}
