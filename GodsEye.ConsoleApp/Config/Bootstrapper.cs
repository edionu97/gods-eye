using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.Utility.Configuration.Configuration;
using GodsEye.Utility.Configuration.Configuration.Impl;

namespace GodsEye.ConsoleApp.Config
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
                            path: Path.Combine(nameof(Config), "appSettings.json"));
                })
                .ConfigureServices((context, services) =>
                {
                    //register the app settings
                    services
                        .AddSingleton<IApplicationSettings>(context.Configuration.Get<ApplicationSettings>());

                })
                .Build()
                .Services;
        }
    }
}
