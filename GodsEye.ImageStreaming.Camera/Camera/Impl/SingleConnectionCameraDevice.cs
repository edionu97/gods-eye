using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GodsEye.Utility.Exceptions;
using GodsEye.Utility.Configuration;
using GodsEye.Utility.Helpers.Network;
using GodsEye.ImageStreaming.Camera.Messages;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;
using LocalConstants = GodsEye.Utility.Constants.Message.MessageConstants.CameraDevice;

namespace GodsEye.ImageStreaming.Camera.Camera.Impl
{
    public class SingleConnectionCameraDevice : ICameraDevice
    {
        private readonly IImageProvider _imageProvider;
        private readonly IApplicationSettings _applicationSettings;

        public SingleConnectionCameraDevice(IImageProvider imageProvider, IApplicationSettings applicationSettings)
        {
            _imageProvider = imageProvider;
            _applicationSettings = applicationSettings;
        }

        public Task StartSendingImageFrames(string deviceId, int devicePort)
        {
            //get the starting point and the camera address
            var (_, _, cameraAddress) = _applicationSettings.Camera.Network;

            //create a new task on thread pool
            return Task.Run(async () =>
            {
                //parse the address 
                var ipAddress = IPAddress.Parse(cameraAddress);

                //get the listener
                var clientListener = new TcpListener(ipAddress, devicePort);

                //start the client listener
                clientListener.Start();

                //display the message
                Console.WriteLine(LocalConstants.CameraIsStreamingMessage, deviceId, devicePort);

                //send the images frame by frame to client
                try
                {
                    //wait for a single connection
                    var client = await clientListener.AcceptSocketAsync();

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
                    throw new CameraDisconnectedException(e.Message);
                }
            });
        }

        private async Task WaitFrameInterval()
        {
            //get the number of frames
            var numberOfFrames =
                _applicationSettings.Camera.ImageOptions.FramesPerSecond;

            //get the frame interval
            var frameInterval =
                Math.Ceiling(1000.0 / numberOfFrames);

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
                ImageType = _applicationSettings.Camera.Network.ImageStreamingFormat
            };

            //send the message to the client
            SendHelpers.SendMessage<ImageFrameMessage>(imageFrameMessage, client);
        }
    }
}
