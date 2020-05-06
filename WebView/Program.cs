using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebView
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    //TODO: create an extension so this isn't copypasta'd
                    var env = hostingContext.HostingEnvironment;
                    string prodFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sharedSettings.json");
                    string environmentFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"sharedSettings.{env.EnvironmentName}.json");
                    configuration.AddJsonFile(prodFilePath);
                    configuration.AddJsonFile(environmentFilePath);
                    configuration.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
