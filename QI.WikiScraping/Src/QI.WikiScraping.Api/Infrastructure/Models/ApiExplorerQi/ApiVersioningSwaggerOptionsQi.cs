using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace QI.WikiScraping.Api.Infrastructure.Models.ApiExplorerQi
{
    /// <summary>
    ///  Option pattern to apply configurations based Versioning and OpenApi implementation
    /// </summary>
    public class ApiVersioningSwaggerOptionsQi
    {
        /// <summary>
        /// The assembly containing the  metadata of ApiProject that is wanted to be exposed.
        /// </summary>
        public Assembly ApiAssembly { get; set; } = null;
        #region Versionning
        /// <summary>
        /// This means, for OpenApi versioning , it will looks for a letter 'v', followed by major and minor version. IE: v1, v1.0, v2, v2.0, etc.
        /// </summary>
        public string GroupNameFormat { get; set; } = "'v'VV";

        /// <summary>
        /// Gets or sets a value indicating whether a default version is assumed when a client
        /// does does not provide a service API version.</summary>
        public bool AssumeDefaultVersionWhenUnspecified { get; set; } = true;

        /// <summary>
        /// Gets or sets the default API version applied to services that do not have explicit
        /// versions.</summary>
        public ApiVersion DefaultApiVersion { get; set; } = new ApiVersion(1, 0);

        /// <summary>
        /// Gets or sets a value indicating whether requests report the service API version
        /// compatibility information in responses.
        /// </summary>
        /// <remarks>
        /// When this property is set to true, the HTTP headers "api-supported-versions"
        ///and "api-deprecated-versions" will be added to all valid service routes.This
        ///information is useful for advertising which versions are supported and scheduled
        ///for deprecation to clients.This information is also useful when supporting the
        ///OPTIONS verb.
        ///By setting this property to true, the Microsoft.AspNetCore.Mvc.ReportApiVersionsAttribute
        ///will be added a global action filter. To enable more granular control over when
        ///service API versions are reported, apply the Microsoft.AspNetCore.Mvc.ReportApiVersionsAttribute
        ///on specific controllers or controller actions.
        ///
        /// </remarks>
        public bool ReportApiVersions { get; set; } = true;
        #endregion

        #region Swagger


        /// <summary>
        /// Dictionary with the Key/Value pairs related to Headers and Values of those headers that we want to wxpose to swagger UI.
        /// </summary>
        /// <code>
        ///     var headerSamplesKeyValues = new Dictionary<string, object>() {
        ///                                             { "HeaderKey1", "HeaderValue1" },
        ///                                             { "HeaderKey2", "HeaderValue2" },
        ///                                             { "HeaderKey3", "HeaderValue3" },
        ///                                             { "HeaderKey4", "HeaderValue4" }
        ///                                 };
        /// </code>
        public Dictionary<string, object> HeaderSamplesKeyValues { get; set; } = null;

        /// <summary>
        /// OpenApi contact Information
        /// </summary>
        public OpenApiContact SwaggerContactOpenApiInfo { get; set; } = null;

        /// <summary>
        /// OpenApi License Information
        /// </summary>
        public OpenApiLicense SwaggerLicenseOpenApiInfo { get; set; } = null;

        /// <summary>
        /// OpenApi Term Of Service Information
        /// </summary>
        public Uri SwaggerTermOfServiceOpenApiInfo { get; set; } = null;

        #endregion
    }
}
