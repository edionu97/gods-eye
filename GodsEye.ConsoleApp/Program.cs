using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GodsEye.ConsoleApp.Config;
using GodsEye.ImageStreaming.Camera.Camera;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodsEye.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //load the service provider
            var serviceProvider = Bootstrapper.Load();

            var configuration =
                serviceProvider.GetService<IApplicationSettings>();

            var onCameras = new List<Task>();

            //iterate the image directory
            var imageDirectory
                = new DirectoryInfo(configuration?.Resources.ImageDirectoryPath ?? string.Empty);

            //get the index of the camera
            var cameraIdx = configuration?.Camera.Network.StreamsOnPort ?? 0;
            foreach (var subDirectory in imageDirectory.GetDirectories(configuration?.Camera.CameraId ?? string.Empty))
            {
                //get the camera device
                var cameraDevice = 
                    serviceProvider.GetService<ICameraDevice>();

                //start the image streaming
                onCameras.Add(cameraDevice?.StartSendingImageFrames(subDirectory.Name, cameraIdx++));
            }

            //wait all the tasks to complete
            Task.WaitAll(onCameras.Where(x => x != null).ToArray());
        }
    }
}
