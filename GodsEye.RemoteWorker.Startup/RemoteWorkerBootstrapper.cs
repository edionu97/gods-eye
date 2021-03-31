using System;
using System.IO;
using GodsEye.RemoteWorker.WebSocket.Server;
using GodsEye.RemoteWorker.WebSocket.Server.Impl;
using GodsEye.RemoteWorker.Worker.Streaming;
using GodsEye.RemoteWorker.Worker.Streaming.Impl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.Utility.Application.Config.Configuration;
using GodsEye.Utility.Application.Config.Configuration.Impl;
using GodsEye.Utility.Application.Config.Settings;
using GodsEye.Utility.Application.Config.Settings.Camera;
using GodsEye.Utility.Application.Config.Settings.Impl;
using GodsEye.Utility.Application.Config.Settings.RemoteWorker;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Security.Encryption.Impl;
using GodsEye.Utility.Application.Security.KeyProvider;
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
                    #region AppConfig + AppSettings

                    //register the app config
                    services.AddSingleton<IAppConfig>(
                        context.Configuration.Get<AppConfig>());

                    //register the camera settings as singleton
                    services.AddSingleton<IApplicationSettings, ApplicationSettings>();

                    //service register the web socket settings
                    services.AddSingleton<IWebSocketSettings>(
                        x => x.GetService<IApplicationSettings>());

                    //service register the web socket settings
                    services.AddSingleton<IWebSocketSettings>(
                        x => x.GetService<IApplicationSettings>());

                    //service register the web socket settings
                    services.AddSingleton<ICameraSettings>(
                        x => x.GetService<IApplicationSettings>());

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

                    //register the web socket server
                    services
                        .AddTransient<IWebSocketServer, BroadcastToAllClientsWebSocketServer>();

                    //register the streaming image worker
                    services
                        .AddTransient<IStreamingImageWorker, StreamingImageWorker>();

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
