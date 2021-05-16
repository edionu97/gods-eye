using System;
using System.Collections.Generic;
using EasyNetQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using GodsEye.Application.Api.Config;
using GodsEye.Application.Middleware.MessageBroadcaster;
using GodsEye.Application.Middleware.MessageBroadcaster.Impl;
using GodsEye.Application.Middleware.WorkersMaster;
using GodsEye.Application.Middleware.WorkersMaster.Impl;
using GodsEye.Application.Persistence.DatabaseContext;
using GodsEye.Application.Services.ImageManipulator;
using GodsEye.Application.Services.ImageManipulator.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Config.Configuration.Sections.RabbitMq;
using Microsoft.EntityFrameworkCore;
using WatsonWebsocket;

namespace GodsEye.Application.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //add the controllers
            services.AddControllers();

            //add the swagger 
            services.AddSwaggerGen();

            //get the application config
            var applicationConfig = Configuration
                .GetSection(nameof(ApplicationConfig))
                .Get<ApplicationConfig>();

            //register the app config as IConfig
            services.AddSingleton<IConfig>(applicationConfig);

            //register the rabbit mq server
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

            //register the message broadcast middleware
            services
                .AddSingleton<IMessageBroadcasterMiddleware, MessageBroadcasterMiddleware>();

            //register the web socket server
            services.AddSingleton(serviceProvider =>
            {
                //get the mq config
                var (address, port) = 
                    serviceProvider
                       .GetService<IConfig>()
                       ?.Get<WsServerConfig>()
                    ?? throw new ArgumentException();

                //create the server 
                var server = new WatsonWsServer(address, port);

                //start the server
                server.Start();

                //return the server instance
                return server;
            });

            //add the master middleware
            services.AddSingleton<IWorkersMasterMiddleware, WorkersMasterMiddleware>();

            //add the service
            services
                .AddSingleton<IFacialImageManipulatorService, FacialImageManipulatorService>();

            //register the database context
            var connectionString = Configuration.GetConnectionString("GodsEyeDb");

            //register as scoped (same instance on the same http request)
            //the connection string will be passed to the database context when executing the update-database command
            services.AddScoped(x => new GodsEyeDatabaseContext(connectionString));


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //add the swagger middleware
            app.UseSwagger();
            
            //swagger is on http://localhost:53361/swagger/index.html
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestAPI data");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
