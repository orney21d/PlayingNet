using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QI.WikiScraping.Api.Infrastructure.ConfigSettings
{
    /// <summary>
    /// Suagger config section
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Virtual path of the hosted site, needed when the site is deployed in order to find swagger file specification .json
        /// </summary>
        public string VirtualPath { get; set; } = string.Empty;

        /// <summary>
        /// Open Api Contact information
        /// </summary>
        public QiOpenApiContact OpenApiContact { get; set; }

        /// <summary>
        /// Open Api License information
        /// </summary>
        public QiOpenApiLicense OpenApiLicense { get; set; }

        /// <summary>
        /// Allianz OpenApi term of service
        /// </summary>
        public string OpenApiTermOfServiceInfoUrl { get; set; } = string.Empty;

    }


    /// <summary>
    /// Handle OpenApi contact information
    /// </summary>
    public class QiOpenApiContact
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

    }

    /// <summary>
    /// Handle OpenApi license data
    /// </summary>
    public class QiOpenApiLicense
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
