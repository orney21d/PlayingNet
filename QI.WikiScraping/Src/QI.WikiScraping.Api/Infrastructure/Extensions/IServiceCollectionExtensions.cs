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
using QI.WikiScraping.Api.Application.Services;
using QI.WikiScraping.Api.Infrastructure.Utils.Validators;
using System.Diagnostics.Contracts;
using System.Reflection;
using QI.WikiScraping.Api.Infrastructure.Models.ApiExplorerQi;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// IServiceCollection extensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Add swagger and versioning service dependencies. This can go to a Nuget package
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerVersioningApiQi<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class
        {

            var suaggerConfig = configuration.GetSection(QiConfigurationSections.Swagger);
            var swaggerConfigValues = new SwaggerConfig();
            suaggerConfig.Bind(swaggerConfigValues);

            //Versioning and OpenApi
            services.AddVersioningAndOpenApiSwashbuckleQi<T>(new ApiVersioningSwaggerOptionsQi
            {
                ApiAssembly = typeof(SwaggerConfig).Assembly,
                SwaggerContactOpenApiInfo = new OpenApiContact()
                {
                    Name = swaggerConfigValues?.OpenApiContact?.Name, //".Net Team",
                    Email = swaggerConfigValues?.OpenApiContact?.Email,//  "NetTeamRangers@allianz.ie",
                    Url = !string.IsNullOrWhiteSpace(swaggerConfigValues?.OpenApiContact?.Url) ? new Uri(swaggerConfigValues.OpenApiContact.Url) : null
                    //, Extensions =  To add more features not supported in OpenAPi specifications like Custom Logos, Headers, etc. 
                },
                SwaggerLicenseOpenApiInfo = new OpenApiLicense()
                {
                    Name = swaggerConfigValues?.OpenApiLicense?.Name,//"Some Allianz Ireland License",
                    Url = !string.IsNullOrWhiteSpace(swaggerConfigValues?.OpenApiLicense?.Url) ? new Uri(swaggerConfigValues.OpenApiLicense.Url) : null  //"https://opensource.org/licenses/MIT")
                },
                SwaggerTermOfServiceOpenApiInfo = !string.IsNullOrWhiteSpace(swaggerConfigValues?.OpenApiTermOfServiceInfoUrl) ? new Uri(swaggerConfigValues.OpenApiTermOfServiceInfoUrl) : null//new Uri("http://www.someTermOfServiceUrl.com")

            });


            return services;
        }

        /// <summary>
        /// Adds an API explorer that is API version aware and Swagger implementation through Swashbuckle.
        /// </summary>
        /// <typeparam name="T">The type of any Class defined in the assembly which have the  'Request/Response' samples for Swagger.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection">services</see> available in the application.</param>
        /// <param name="options">An <see cref="Action{T}">action</see> used to configure the provided options.</param>
        /// <returns>The original <paramref name="services"/> object.</returns>
        public static IServiceCollection AddVersioningAndOpenApiSwashbuckleQi<T>(this IServiceCollection services, ApiVersioningSwaggerOptionsQi options)
            where T : class

        {
            QiArg.NotNull(services, nameof(services));
            QiArg.NotNull(options, nameof(options));
            QiArg.NotNull(options.ApiAssembly, nameof(options.ApiAssembly));
            Contract.Ensures(Contract.Result<IServiceCollection>() != null);

            string swaggerOpenApiDocNamePrefix = $"{Assembly.GetExecutingAssembly().GetName().Name}".Replace(".", "") + "OpenAPISpecification";
            #region Versionning

            services.AddVersionedApiExplorer(setupAction =>
            {
                setupAction.GroupNameFormat = options.GroupNameFormat; /*This means, for OpenApi versioning , it will looks for a letter 'v', 
                                                        followed by major and minor version. IE: v1, v1.0, v2, v2.0, etc...*/
            })
            .AddApiVersioning(setupAction => {
                setupAction.AssumeDefaultVersionWhenUnspecified = options.AssumeDefaultVersionWhenUnspecified;
                setupAction.DefaultApiVersion = options.DefaultApiVersion;
                setupAction.ReportApiVersions = options.ReportApiVersions;  //Send in the response the supported version
            });

            //Below line should be called after call "services.AddApiVersioning" else we'll end up with one version cause no other versions has been applied.
            var apiVersionDescriptionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            var title = options.ApiAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

            var description = options.ApiAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;

            #endregion

            #region Swagger 

            //Handle the  swagger doc 
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.ExampleFilters();

                #region Header Request Examples


                if (options.HeaderSamplesKeyValues != null && options.HeaderSamplesKeyValues.Any())
                {
                    foreach (var item in options.HeaderSamplesKeyValues)
                    {
                        setupAction.OperationFilter<AddHeaderOperationFilter>(item.Key, item.Value);
                    }
                }


                #endregion



                foreach (var apiDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerDoc(
                    $"{swaggerOpenApiDocNamePrefix}{apiDescription.GroupName}",
                    new OpenApiInfo()
                    {
                        Title = $"{title} {apiDescription.ApiVersion}",
                        Version = apiDescription.ApiVersion.ToString(),
                        Description = apiDescription.IsDeprecated ? $"{description} - DEPRECATED" : description,
                        Contact = options.SwaggerContactOpenApiInfo ?? null,
                        License = options.SwaggerLicenseOpenApiInfo ?? null,
                        TermsOfService = options.SwaggerTermOfServiceOpenApiInfo ?? null

                    });
                }

                //To link Actions with the corresponding version of the OpenApi
                setupAction.DocInclusionPredicate((documentName, apiDescription) =>
                {
                    var actionApiVersionModel = apiDescription.ActionDescriptor.GetApiVersionModel(
                        ApiVersionMapping.Explicit  //This means , Action with the ApiVersion attribute
                        | ApiVersionMapping.Implicit //This means , Action with the default version

                        );

                    if (actionApiVersionModel == null)
                        return true;

                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                            $"{swaggerOpenApiDocNamePrefix}v{v.ToString()}" == documentName);

                    }

                    return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                            $"{swaggerOpenApiDocNamePrefix}v{v.ToString()}" == documentName);

                });

                //Custom schemaIds to avoid conflicts between types.
                setupAction.CustomSchemaIds(ctype => ctype.FullName);

                //getting the name of .Xml comments document.
                string xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                //Get xml full path
                string xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                //If exists the comment file we'll add it
                if (File.Exists(xmlCommentsFullPath))
                {
                    setupAction.IncludeXmlComments(xmlCommentsFullPath);
                }

            });

            //Specifying the assembly where goes the Swagger samples "Request / Response"
            services.AddSwaggerExamplesFromAssemblyOf<T>();
            #endregion

            return services;
        }


        /// <summary>
        /// Register MVC core services dependencies
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMvcCoreDependenciesQi(this IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                //Ini registering global filters for Swagger response tagging
                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));

                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));

                options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                options.Filters.Add(typeof(HttpGlobalExceptionFilter)); //Handling the exceptions
                options.Filters.Add(typeof(QiValidModelStateFilterAttribute)); //Validate the model

            })

           .AddApiExplorer()
           .AddFormatterMappings()
           //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
           ;

            return services;
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
        public static IServiceCollection AddHealthChecksQi(this IServiceCollection services)
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
        public static IServiceCollection AddConfigDependenciesQi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            //services.Configure<UrlsConfig>(configuration.GetSection(QiConfigurationSections.Urls));

            return services;
        }

    }
}
