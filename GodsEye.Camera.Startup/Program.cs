using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GodsEye.Camera.ImageStreaming.Camera;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration;
using GodsEye.Utility.Application.Config.Configuration.Sections.Camera;
using Microsoft.Extensions.DependencyInjection;

namespace GodsEye.Camera.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //load the service provider
            var serviceProvider = Bootstrapper.Load();

            //the app config
            var configuration =
                serviceProvider.GetService<IConfig>();

            //check if the configuration is null
            if (configuration == null)
            {
                return;
            }

            //get the image directory path
            var (imageDirectoryPath, cameraId) = configuration.Get<CameraSectionConfig>();

            //get the camera port
            var (_, cameraPort, _) = configuration.Get<NetworkSectionConfig>();

            //iterate the image directory
            var imageDirectory = new DirectoryInfo(imageDirectoryPath);

            //get the index of the camera
            var onCameras = new List<Task>();
            foreach (var subDirectory in imageDirectory.GetDirectories(cameraId))
            {
                //get the camera device
                var cameraDevice = 
                    serviceProvider.GetService<ICameraDevice>();

                //start the image streaming
                onCameras.Add(cameraDevice?.StartSendingImageFrames(subDirectory.Name, cameraPort++));
            }

            //wait all the tasks to complete
            Task.WaitAll(onCameras.Where(x => x != null).ToArray());
        }
    }
}
