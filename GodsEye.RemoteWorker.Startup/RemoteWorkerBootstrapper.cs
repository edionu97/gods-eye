using System;
using System.Collections.Generic;
using System.IO;
using EasyNetQ;
using GodsEye.RemoteWorker.Startup.StartupWorker;
using GodsEye.RemoteWorker.Startup.StartupWorker.Impl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using GodsEye.RemoteWorker.Worker.Streaming;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Worker.Streaming.Impl;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Security.KeyProvider;
using GodsEye.Utility.Application.Security.Encryption.Impl;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket.Impl;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Impl;
using GodsEye.Utility.Application.Security.KeyProvider.Impl;

namespace GodsEye.RemoteWorker.Startup
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

                    //register the service provider
                    services.AddSingleton(s => s);

                    //register the web socket server
                    services
                        .AddTransient<IWebSocketServer, JsonBroadcastWebSocketServer>();

                    //register the streaming image worker
                    services
                        .AddTransient<IStreamingImageWorker, StreamingImageWorker>();

                    //register the worker starter
                    services
                        .AddSingleton<IMessageQueueRemoteWorkerStarter, MessageQueueRemoteWorkerStarter>();
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
