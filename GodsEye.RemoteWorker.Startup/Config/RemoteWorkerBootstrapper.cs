using System;
using System.Collections.Generic;
using System.IO;
using EasyNetQ;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies;
using GodsEye.DataStreaming.LoadShedding.LoadSheddingPolicies.Impl;
using GodsEye.DataStreaming.LoadShedding.Manager;
using GodsEye.DataStreaming.LoadShedding.Manager.Impl;
using GodsEye.RemoteWorker.Startup.StartupWorker;
using GodsEye.RemoteWorker.Startup.StartupWorker.Impl;
using GodsEye.RemoteWorker.Worker.FacialAnalysis;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.GrpcProxy.Impl;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.Impl;
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
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Items.Statistics.Time.ElapsedTime;
using GodsEye.Utility.Application.Items.Statistics.Time.ElapsedTime.Impl;
using GodsEye.Utility.Application.Resources.Manager;
using GodsEye.Utility.Application.Resources.Manager.Impl;
using GodsEye.Utility.Application.Security.Encryption;
using GodsEye.Utility.Application.Security.Encryption.Impl;
using GodsEye.Utility.Application.Security.KeyProvider;
using GodsEye.Utility.Application.Security.KeyProvider.Impl;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RemoteWorkerImpl = GodsEye.RemoteWorker.Worker.Remote.Impl.RemoteWorker;

using static Gods.Eye.Server.Artificial.Intelligence.Messaging.FacialRecognitionAndAnalysis;

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

                    //register the grpc channel with it's credentials
                    services.AddTransient<ChannelBase>(serviceProvider =>
                    {
                        //get the app config
                        var grpcConfig = serviceProvider
                                             .GetService<IConfig>()
                                             ?.Get<GrpcFacialAnalysisServerConfig>()
                                         ?? throw new ArgumentException();

                        //destruct the application config
                        var (certificateLocation, host, port) = grpcConfig;

                        //read the certificate bytes
                        var channelSslCredentials = new SslCredentials(File.ReadAllText(certificateLocation));

                        //create the new channel
                        return new Channel(host, port, channelSslCredentials);
                    });

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

                    //register the resources manager
                    services.AddSingleton<IResourcesManager, ResourcesManager>();

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

                    //add the period time interval counter
                    services
                        .AddTransient<IPeriodTimeIntervalCounter, LoopPeriodTimeIntervalIntervalCounter>();

                    //register the streaming image worker
                    services
                        .AddTransient<IStreamingImageWorker, StreamingImageWorker>();

                    //add the grpc recognition client 
                    services.AddTransient<FacialRecognitionAndAnalysisClient>();

                    //add the facial recognition and analysis service
                    services
                        .AddTransient<IFacialRecognitionAndAnalysisService, FacialRecognitionAndAnalysisService>();

                    //add the facial analysis and recognition worker
                    services
                        .AddTransient<IFacialAnalysisAndRecognitionWorker, FacialAnalysisAndRecognitionWorker>();

                    //register the remote worker
                    services.AddTransient<IRemoteWorker, RemoteWorkerImpl>();

                    //register the load shedding policies
                    services
                        .AddSingleton<NoLoadSheddingPolicy>();
                    services
                        .AddSingleton<RandomLoadSheddingPolicy>();
                    services
                        .AddSingleton<HeuristicLoadSheddingPolicy>();

                    //register also the no load shedding policy as INoLoadSheddingPolicy
                    services.AddSingleton<INoLoadSheddingPolicy>(serviceProvider =>
                        serviceProvider.GetService<NoLoadSheddingPolicy>());

                    //register the used load shedding policy
                    services.AddSingleton<ILoadSheddingPolicy>(serviceProvider =>
                    {
                        //get the configuration
                        var configuration =
                            serviceProvider
                                .GetService<IConfig>()
                            ?? throw new ArgumentNullException();

                        //get the load shedding policy
                        var sheddingPolicy = configuration
                            .Get<FacialAnalysisAndRecognitionWorkerConfig>();

                        //return the instance 
                        return sheddingPolicy?.LoadSheddingPolicy switch
                        {
                            //handle the no load shedding action
                            LoadSheddingPolicyType.NoLoadShedding => serviceProvider.GetService<NoLoadSheddingPolicy>(),

                            //handle the random load shedding policy
                            LoadSheddingPolicyType.RandomLoadShedding => serviceProvider.GetService<RandomLoadSheddingPolicy>(),

                            //handle the case of heuristic load shedding
                            LoadSheddingPolicyType.HeuristicLoadShedding => serviceProvider.GetService<HeuristicLoadSheddingPolicy>(),

                            //throw exception if the policy is not known
                            _ => throw new ArgumentOutOfRangeException(sheddingPolicy?.LoadSheddingPolicy.ToString())
                        };
                    });

                    //register the policy manager
                    services
                        .AddSingleton<ILoadSheddingFixedPolicyManager, LoadSheddingFixedPolicyManager>();

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

                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .Build()
                .Services;
        }
    }
}
