using System;
using System.Collections.Generic;
using System.IO;
using EasyNetQ;
using GodsEye.Camera.ImageStreaming.Camera;
using GodsEye.Camera.ImageStreaming.Camera.Impl;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageLocator;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageLocator.Impl;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider;
using GodsEye.Camera.ImageStreaming.ImageSource.ImageProvider.Impl;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Impl;
using GodsEye.Utility.Application.Config.Configuration.Sections.RabbitMq;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Security.Encryption.Impl;
using GodsEye.Utility.Application.Security.KeyProvider;
using GodsEye.Utility.Application.Security.KeyProvider.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GodsEye.Camera.Startup.Config
{
    public static class CameraConfiguration
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
                    #region AppConfig

                    //register the app config
                    services.AddSingleton<IConfig>(context.Configuration.Get<AppConfig>());

                    #endregion

                    #region Authentication

                    //register the key provider
                    services.AddSingleton<KeyBasicHashProvider>();

                    //register the IKeyProvider
                    services.AddSingleton<IKeyProvider, KeyBasicHashProvider>(s =>
                    {
                        //get the basic provider
                        var basicHashProvider = s.GetService<KeyBasicHashProvider>();

                        //register the key
                        basicHashProvider?.RegisterKey("app key");

                        //get the provider
                        return basicHashProvider;
                    });

                    #endregion

                    //register the rabbit mq
                    services.AddSingleton(serviceProvider =>
                    {
                        //get the mq config
                        var mqConfig = serviceProvider
                            .GetService<IConfig>()
                            ?.Get<RabbitMqConfig>();

                        //destruct the message or throw exception
                        var (username, password, host, port) =
                            mqConfig
                            ?? throw new ArgumentNullException(nameof(RabbitMqConfig));

                        //create the queue
                        return RabbitHutch.CreateBus(
                            new ConnectionConfiguration
                            {
                                UserName = username,
                                Password = password,
                                Hosts = new List<HostConfiguration>
                                {
                                    new HostConfiguration
                                    {
                                        Host = host,
                                        Port = port
                                    }
                                }
                            },
                            _ => { });
                    });

                    //register the encryptor as singleton
                    services
                        .AddSingleton<IEncryptorDecryptor, KeyBasedEncryptorDecryptor>();

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
                    logging.AddSimpleConsole(option =>
                    {
                        option.IncludeScopes = true;
                        option.TimestampFormat = "[HH:mm:ss] ";
                    });
                })
                .Build()
                .Services;
        }
    }
}
