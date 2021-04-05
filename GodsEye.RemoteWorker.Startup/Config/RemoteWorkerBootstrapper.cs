using System;
using System.Collections.Generic;
using System.IO;
using EasyNetQ;
using GodsEye.RemoteWorker.Startup.StartupWorker;
using GodsEye.RemoteWorker.Startup.StartupWorker.Impl;
using GodsEye.RemoteWorker.Worker.Remote;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer;
using GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer.Impl;
using GodsEye.RemoteWorker.Worker.Streaming.Impl;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket.Impl;
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

namespace GodsEye.RemoteWorker.Startup.Config
{


    public static class RemoteWorkerBootstrapper
    {
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
                    services.AddSingleton<IConfig>(
                        context.Configuration.Get<AppConfig>());

                    #endregion

                    #region Authorization

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

                    //register the encryptor as singleton
                    services
                        .AddSingleton<IEncryptorDecryptor, KeyBasedEncryptorDecryptor>();

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

                    //register the service provider
                    services.AddSingleton(s => s);

                    //register the web socket server
                    services
                        .AddTransient<IWebSocketServer, JsonBroadcastWebSocketServer>();

                    //add the frame buffer
                    services
                        .AddTransient<IFrameBuffer, ReverseOrderFrameBuffer>();

                    //register the streaming image worker
                    services
                        .AddTransient<IStreamingImageWorker, StreamingImageWorker>();

                    services
                        .AddTransient<IRemoteWorker, Worker.Remote.Impl.RemoteWorker>();

                    //register the worker starter
                    services
                        .AddSingleton<IMessageQueueRemoteWorkerStarter, MessageQueueRemoteWorkerStarter>();

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
