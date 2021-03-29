using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Settings;
using LocalConstants = GodsEye.Utility.Constants.Message.MessageConstants.CameraDevice;

namespace GodsEye.ImageStreaming.Camera.Camera.Impl
{
    public partial class SingleConnectionCameraDevice : ICameraDevice
    {
        public SingleConnectionCameraDevice(IImageProvider imageProvider, ICameraSettings cameraSettings)
        {
            _imageProvider = imageProvider;
            _cameraSettings = cameraSettings;
        }

        public Task StartSendingImageFrames(string deviceId, int devicePort)
        {
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
                    Console.WriteLine(LocalConstants.CameraIsStreamingMessage, deviceId, devicePort);

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

                        //write 
                        Console.WriteLine(LocalConstants.CameraIsStreamingImagesMessage, message);

                        //iterate through the available images
                        await foreach (var frameInfo in _imageProvider.ProvideImages(deviceId))
                        {
                            //send frame
                            SendFrameToClient(client, frameInfo);

                            //wait the interval in order to match the desired fps
                            await WaitFrameInterval();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                // ReSharper disable once FunctionNeverReturns
            });
        }
    }
}
