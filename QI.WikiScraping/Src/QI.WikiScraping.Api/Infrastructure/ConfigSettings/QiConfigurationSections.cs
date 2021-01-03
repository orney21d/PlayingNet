using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QI.WikiScraping.Api.Infrastructure.ConfigSettings
{
    /// <summary>
    /// Define the sections to handle in the settings
    /// </summary>
    public sealed class QiConfigurationSections
    {
        /// <summary>
        /// Logging
        /// </summary>
        public const string Logging = "Logging";

        /// <summary>
        /// Swagger section config values
        /// </summary>
        public const string Swagger = "Swagger";
    }
}
