using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using GodsEye.RemoteWorker.Worker.Streaming;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Worker.Streaming.Impl;
using GodsEye.Utility.Application.Config.Settings;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Security.KeyProvider;
using GodsEye.Utility.Application.Config.Configuration;
using GodsEye.Utility.Application.Config.Settings.Impl;
using GodsEye.Utility.Application.Config.Settings.Camera;
using GodsEye.Utility.Application.Security.Encryption.Impl;
using GodsEye.RemoteWorker.Worker.Streaming.WebSocket.Impl;
using GodsEye.Utility.Application.Config.Configuration.Impl;
using GodsEye.Utility.Application.Security.KeyProvider.Impl;
using GodsEye.Utility.Application.Config.Settings.RemoteWorker;

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
                        .AddTransient<IWebSocketServer, JsonBroadcastWebSocketServer>();

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
