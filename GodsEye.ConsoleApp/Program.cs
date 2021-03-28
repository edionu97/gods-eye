using GodsEye.ConsoleApp.Config;
using GodsEye.Utility.Configuration.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodsEye.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //load the service provider
            var serviceProvider = Bootstrapper.Load();

            var configuration =
                serviceProvider.GetService<IApplicationSettings>();


        }
    }
}
