using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QI.WikiScraping.Api.Infrastructure.Filters;

namespace QI.WikiScraping.Api
{
    /// <summary>
    /// Define the registration process WebApi's related
    /// </summary>
    public class ApiConfiguration
    {

        public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddControllers().AddNewtonsoftJson(); //Better to use TextJson for performance improvements but for the QI test this is ok.
            services.AddQiHealthChecks()
                .AddQiConfigDependencies(configuration); //Adding the configuration binding dependencies we could have


            #region Http Client Factory

            services.AddHttpClientsFactoryQi(configuration);

            #endregion


           



            return services;
        }

        public static IApplicationBuilder Configure(
            IApplicationBuilder app,
            Func<IApplicationBuilder, IApplicationBuilder> configureHost,
            IWebHostEnvironment env,
             IApiVersionDescriptionProvider apiVersionDescriptionProvider
            )
        {



            return configureHost(app)
                .UseRouting();   
                
        }
    }
}
