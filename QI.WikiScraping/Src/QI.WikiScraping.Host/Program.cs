using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QI.WikiScraping.Api.Infrastructure.ConfigSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostingAsp = Microsoft.Extensions.Hosting;

namespace QI.WikiScraping.Host
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {

                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {

                return 1;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             HostingAsp.Host.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webbuilder =>
                {
                    webbuilder
                        .UseIISIntegration()
                        .UseStartup<Startup>();
                })
            //Configuring Logging Provider
            .ConfigureLogging((hostingContext, logging) =>
            {

                //For the purpose of the test I'm adding the Console and Debug logger provider but in a real Project I would
                //add those providers in Development environment, not in Production
                logging.AddConfiguration(hostingContext.Configuration.GetSection(QiConfigurationSections.Logging));
                logging.AddConsole();
                logging.AddDebug();
            })

               ;
    }
}
