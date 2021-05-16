using System;
using System.Collections.Generic;
using System.IO;
using EasyNetQ;
using GodsEye.Application.Middleware;
using GodsEye.Application.Middleware.MessageBroadcaster;
using GodsEye.Application.Middleware.MessageBroadcaster.Impl;
using GodsEye.Application.Middleware.WorkersMaster;
using GodsEye.Application.Middleware.WorkersMaster.Impl;
using GodsEye.Application.Persistence.DatabaseContext;
using GodsEye.Application.Persistence.Models;
using GodsEye.Application.Persistence.Repository;
using GodsEye.Application.Persistence.Repository.Impl;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Impl;
using GodsEye.Utility.Application.Config.Configuration.Sections.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WatsonWebsocket;

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

                    services.AddSingleton(_ =>
                    {
                        //create the server 
                        var server = new WatsonWsServer();

                        //start the server
                        server.Start();

                        //return the server instance
                        return server;
                    });

                    services.AddSingleton<IMessageBroadcasterMiddleware, MessageBroadcasterMiddleware>();

                    //register as scoped (same instance on the same http request)
                    //the connection string will be passed to the database context when executing the update-database command
                    services.AddScoped(x => new GodsEyeDatabaseContext("Data Source=DESKTOP-VQ4KD11;Initial Catalog=GodsEye;Integrated Security=True"));

                    services.AddScoped<IUserRepository, UserRepository>();

                })
                .Build()
                .Services;
        }
    }
}
