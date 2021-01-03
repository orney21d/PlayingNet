using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QI.WikiScraping.Api.Infrastructure.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQiHttpError
    {
        int Status { get; set; }

        /// <summary>
        /// Status code as string
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// Friendly User messages
        /// </summary>
        string[] UserMessage { get; set; }

        /// <summary>
        /// If is in Dev mode contain a message as is for Developer
        /// </summary>
        string DeveloperMessage { get; set; }

        /// <summary>
        ///
        /// </summary>
        string[] ValidationErrors { get; set; }

    }
}
