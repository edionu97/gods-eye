﻿using System;
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
using GodsEye.Utility.Application.Config.Configuration;
using GodsEye.Utility.Application.Config.Configuration.Impl;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Security.Encryption.Impl;
using GodsEye.Utility.Application.Security.KeyProvider;
using GodsEye.Utility.Application.Security.KeyProvider.Impl;
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
                    services.AddSingleton<IConfig>(context.Configuration.Get<AppConfig>());

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

                    //register the rabbit mq
                    services.AddSingleton(x => RabbitHutch.CreateBus(
                        new ConnectionConfiguration
                        {
                            UserName = "admin",
                            Password = "admin",
                            Hosts = new List<HostConfiguration>
                            {
                                new HostConfiguration
                                {
                                    Host = "192.168.0.101",
                                    Port = 5672
                                }
                            }
                        },
                        _ => { }));

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
                    logging.AddConsole();
                })
                .Build()
                .Services;
        }
    }
}
