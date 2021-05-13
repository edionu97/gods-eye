using System;
using System.Collections.Generic;
using System.IO;
using EasyNetQ;
using GodsEye.Application.Middleware;
using GodsEye.Application.Middleware.Impl;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Impl;
using GodsEye.Utility.Application.Config.Configuration.Sections.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp1.Config
{
    public static class Bootstrapper
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
                    services.AddSingleton<IConfig>(context.Configuration.Get<AppConfig>());

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

                    //add the master middleware
                    services.AddSingleton<IWorkersMasterMiddleware, WorkersMasterMiddleware>();
                })
                .Build()
                .Services;
        }
    }
}
