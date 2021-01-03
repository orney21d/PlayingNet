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
using Microsoft.Extensions.Hosting;
using QI.WikiScraping.Api.Infrastructure.ConfigSettings;

namespace QI.WikiScraping.Api
{
    /// <summary>
    /// Define the registration process WebApi's related
    /// </summary>
    public class ApiStartupConfiguration
    {

        public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddControllers().AddNewtonsoftJson(); //Better to use TextJson for performance improvements but for the QI test this is ok.
            services.AddHealthChecksQi()
                .AddConfigDependenciesQi(configuration); //Adding the configuration binding dependencies we could have


            #region Http Client Factory

            services.AddHttpClientsFactoryQi(configuration);

            #endregion


            services.AddMvcCoreDependenciesQi();


            #region OpenApi-Swagger / Versioning

            services.AddSwaggerVersioningApiQi<ApiStartupConfiguration>(configuration);

            #endregion

            return services;
        }

        public static IApplicationBuilder Configure(
            IApplicationBuilder app,
            Func<IApplicationBuilder, IApplicationBuilder> configureHost,
            IWebHostEnvironment env,
             IApiVersionDescriptionProvider apiVersionDescriptionProvider,
             IConfiguration configuration
            )
        {



            IApplicationBuilder appBuilder = configureHost(app)
                .UseQiHealthChecks();

            if (!env.IsDevelopment())
            {
                appBuilder.UseExceptionHandler("/Error");
                appBuilder.UseHsts();
                appBuilder.UseHttpsRedirection();
            }

            #region Swagger middlewares
             
            appBuilder.UseOpenApiSwashbuckleQi(configuration, apiVersionDescriptionProvider);

            #endregion

            appBuilder.UseRouting();




            return appBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });

        }
    }
}
