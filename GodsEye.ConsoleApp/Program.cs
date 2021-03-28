using System;
using SixLabors.ImageSharp;
using System.Threading.Tasks;
using GodsEye.ConsoleApp.Config;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Utility.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodsEye.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //load the service provider
            var serviceProvider = Bootstrapper.Load();

            var configuration =
                serviceProvider.GetService<IApplicationSettings>();

            var imageStreamer =
                serviceProvider.GetService<IImageProvider>();

            await foreach (var (imageName, imageBytes) in imageStreamer?.ProvideImages("Camera0"))
            {
                await Task.Delay(100);
                var image = Image.Load(imageBytes);
                await image.SaveAsync(@$"C:\Users\Eduard\Desktop\Images\{imageName}");
            }
        }
    }
}
