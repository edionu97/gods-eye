using System;
using System.IO;
using System.Net.Sockets;
using GodsEye.ImageStreaming.Camera.Camera;
using GodsEye.ImageStreaming.Camera.Camera.Impl;
using GodsEye.ImageStreaming.ImageSource.ImageLocator;
using GodsEye.ImageStreaming.ImageSource.ImageLocator.Impl;
using GodsEye.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.ImageStreaming.ImageSource.ImageProvider.Impl;
using GodsEye.Utility.Configuration;
using GodsEye.Utility.Configuration.Base;
using GodsEye.Utility.Configuration.Impl;
using GodsEye.Utility.Settings;
using GodsEye.Utility.Settings.Impl;
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
                    //register the app config
                    services.AddSingleton<IAppConfig>(
                        context.Configuration.Get<AppConfig>());

                    //register the application settings as singleton
                    services.AddSingleton<IApplicationSettings, ApplicationSettings>();

                    //register the camera settings
                    services.AddSingleton<ICameraSettings>(serviceProvider =>
                        serviceProvider.GetService<IApplicationSettings>());

                    //register the image provider as transient (new instance every time)
                    services.AddSingleton<IImageLocator, LocalFileSystemJpegImageLocator>();

                    //register the image provider as transient (new instance every time)
                    services.AddTransient<IImageProvider, ContinuouslyImageProvider>();

                    //register the camera as transient (new instance every time)
                    services.AddTransient<ICameraDevice, SingleConnectionCameraDevice>();
                })
                .Build()
                .Services;
        }
    }
}
