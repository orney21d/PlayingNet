using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QI.WikiScraping.Api.Infrastructure.ConfigSettings;
using QI.WikiScraping.Api.Infrastructure.Filters;
using QI.WikiScraping.Api.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// IServiceCollection extensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Register MVC core services dependencies
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAziMvcCoreDependencies(this IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                //Ini registering global filters for Swagger response tagging
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));

                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));

                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                options.Filters.Add(typeof(AziValidModelStateFilterAttribute));

            })

           .AddApiExplorer()
           .AddFormatterMappings()
           //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
           ;
        }

            /// <summary>
            /// Register and define every client httpClient factory with the Retry,  CircuitBreaker and HttpMessageHandler   for each client
            /// </summary>
            /// <returns></returns>
            public static IServiceCollection AddHttpClientsFactoryQi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IWikiScraperService, WikiScraperService>();
                //I could add Poly retry pattern and Circuit Breaker if it would be an Http service hight demmanded
                
            return services;
        }



        /// <summary>
        /// Register the service to use the HealthChecks
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddQiHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks();
            return services;
        }

        /// <summary>
        /// Add any configuration dependency we can have
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddQiConfigDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            //services.Configure<UrlsConfig>(configuration.GetSection(QiConfigurationSections.Urls));
           
            return services;
        }

    }
}
