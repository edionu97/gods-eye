using System;
using System.IO;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.ImageStreaming.ImageSource.ImageProvider.Impl;
using GodsEye.ImageStreaming.ImageSource.ImageStream;
using GodsEye.ImageStreaming.ImageSource.ImageStream.Impl;
using GodsEye.Utility.Configuration;
using GodsEye.Utility.Configuration.Impl;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodsEye.ConsoleApp.Config
{
    public static class Bootstrapper
    {
        /// <summary>
        /// Configure the classes for dependency injection
        /// </summary>
        /// <returns>the service provider that will be used for getting the instances</returns>
        public static IServiceProvider Load()
        {
            return Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    //mark the location of configuration file
                    builder
                        .AddJsonFile(
                            optional: false,
                            path: Path.Combine(nameof(Config), "appSettings.json"));
                })
                .ConfigureServices((context, services) =>
                {
                    //register the app settings
                    services.AddSingleton<IApplicationSettings>(
                        context.Configuration.Get<ApplicationSettings>());

                    //register the image provider as transient (new instance every time)
                    services.AddSingleton<IImageProvider, LocalFileImageJpegProvider>();

                    //register the image provider as transient (new instance every time)
                    services.AddTransient<IImageStreamer, ContinuouslyImageStreamer>();
                })
                .Build()
                .Services;
        }
    }
}
