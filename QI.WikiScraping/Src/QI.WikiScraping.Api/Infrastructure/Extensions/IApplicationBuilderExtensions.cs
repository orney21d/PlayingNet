using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using QI.WikiScraping.Api.Infrastructure.Utils.Validators;
using QI.WikiScraping.Api.Infrastructure.ConfigSettings;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Add the HealthCheck middleware to PipeLine
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseQiHealthChecks(this IApplicationBuilder appBuilder)
        {

            HealthCheckOptions hchkOptions = new HealthCheckOptions()
            {

                ResponseWriter = async (context, report) =>
                {
                    var result = JsonSerializer.Serialize(
                       new
                       {

                           status = report.Status.ToString(),
                           errors = report.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                       }, new JsonSerializerOptions()
                       {
                           IgnoreNullValues = true,
                           WriteIndented = false
                       });
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            };

            //Setting the response status code for unabailable services as "503"
            hchkOptions.ResultStatusCodes[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable;

            appBuilder.UseHealthChecks("/health", hchkOptions);
            return appBuilder;

        }

        /// <summary>
        /// Having in account some condition we can do something with appBuilder, for example enable "Developer Exception Page"
        /// </summary>
        /// <param name="app"></param>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIf(this IApplicationBuilder app, bool condition, Func<IApplicationBuilder, IApplicationBuilder> action)
        {
            return condition ? action(app) : app;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="swaggerConfigSection"></param>
        /// <param name="apiVersionDescriptionProvider"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOpenApiSwashbuckleQi(this IApplicationBuilder appBuilder, IConfiguration configuration, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            QiArg.NotNull(configuration, nameof(configuration));

            appBuilder.UseSwagger();

            #region Mapping Swagger config section
            var suaggerConfig = configuration.GetSection(QiConfigurationSections.Swagger);
            var swaggerConfigValues = new SwaggerConfig();
            suaggerConfig.Bind(swaggerConfigValues);
            #endregion

            string swaggerOpenApiDocNamePrefix = $"{Assembly.GetExecutingAssembly().GetName().Name}".Replace(".", "") + "OpenAPISpecification";

            appBuilder.UseSwaggerUI(swaggerUiOptions =>
            {
                
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(swaggerConfigValues.VirtualPath) ? string.Empty : $"/{swaggerConfigValues.VirtualPath}";
                swaggerUiOptions.RoutePrefix = string.Empty; //Allow swagger start when app run
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    swaggerUiOptions.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/" +
                        $"{swaggerOpenApiDocNamePrefix}{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
                }
               

                swaggerUiOptions.DefaultModelExpandDepth(2);
                swaggerUiOptions.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                swaggerUiOptions.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });


            return appBuilder;
        }



    }
}
