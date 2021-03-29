using System;
using System.IO;
using GodsEye.Camera.ImageStreaming.Camera;
using GodsEye.Camera.ImageStreaming.Camera.Impl;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageLocator;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageLocator.Impl;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider.Impl;
using GodsEye.Utility.Application.Config.Configuration;
using GodsEye.Utility.Application.Config.Configuration.Impl;
using GodsEye.Utility.Application.Config.Settings;
using GodsEye.Utility.Application.Config.Settings.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GodsEye.Camera.Startup
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
                            path: Path.Combine("ConfigFiles", "appSettings.json"));
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
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .Build()
                .Services;
        }
    }
}
