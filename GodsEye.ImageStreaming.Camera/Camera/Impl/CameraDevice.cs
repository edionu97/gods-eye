using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GodsEye.Utility.Configuration;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;

namespace GodsEye.ImageStreaming.Camera.Camera.Impl
{
    public class CameraDevice : ICameraDevice
    {
        private readonly IImageProvider _imageProvider;
        private readonly IApplicationSettings _applicationSettings;

        public CameraDevice(IImageProvider imageProvider, IApplicationSettings applicationSettings)
        {
            _imageProvider = imageProvider;
            _applicationSettings = applicationSettings;
        }

        public Task StartTackingShots(string deviceId, int devicePort)
        {
            //get the starting point and the camera address
            var (_, _, cameraAddress) = _applicationSettings.Camera.Network;

            //create a new task
            return Task.Factory.StartNew(() =>
            {
                //parse the address 
                var ipAddress = IPAddress.Parse(cameraAddress);

                //get the listener
                var clientListener = new TcpListener(ipAddress, devicePort);

                //start the client listener
                clientListener.Start();

                //display the message
                Console.WriteLine($"Camera {deviceId} is streaming images on port {deviceId}...");

                //wait for connections
                while (true)
                {
                    //get the client socket
                    var client = clientListener.AcceptSocket();

                    //process the message socket
                    OnClientConnected(client, deviceId);
                }

                // ReSharper disable once FunctionNeverReturns
            }, TaskCreationOptions.LongRunning);
        }

        private void OnClientConnected(Socket client, string deviceId)
        {
            Task.Factory.StartNew(async () =>
            {
                await foreach (var (imageName, imageBytes) in _imageProvider.ProvideImages(deviceId))
                {

                }
            });
        }
    }
}
