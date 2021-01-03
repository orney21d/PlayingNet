using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QI.WikiScraping.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QI.WikiScraping.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }


        public IWebHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            #region Host related services
            //Here we can take advantage of the separartion of concern between host and WebApi and before call
            //the WebApi ConfigurationServices we can define the Authentication approach "for instance JWT", also
            //define the ErrorHandling approach (for instance through a middleware to keep clean the WebApi project in term of handling exceptions) 
            #endregion

            #region WebApi related services

            //Using separation of conserns by solution architecture. This is good for instance for isolate Unit Tests, etc.

            ApiStartupConfiguration.ConfigureServices(services, Configuration, Env);

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            //Here we can call the middleware for authentication, that way we give to the host the responsability to register
            //Auth stuff.

            ApiStartupConfiguration.Configure(
                app,
                host => host
                .UseIf(env.IsDevelopment(), appBuilder => appBuilder.UseDeveloperExceptionPage()),
                env,
                apiVersionDescriptionProvider,
                Configuration
            );

        }
    }
}
