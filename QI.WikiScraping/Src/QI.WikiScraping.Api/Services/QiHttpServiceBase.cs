using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QI.WikiScraping.Api.Infrastructure;

namespace QI.WikiScraping.Api.Services
{
    //This can be defined in a Nuget package to be used accross projects

    /// <summary>
    /// Http service base to handle the http response or throw an exception <seealso cref="RethrowApiException"/> if
    /// the result is not in the margin of 200-209
    /// </summary>
    public abstract class QiHttpServiceBase
    {
        /// <summary>
        /// logger to trace the data
        /// </summary>
        protected readonly ILogger<QiHttpServiceBase> _logger;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public QiHttpServiceBase(ILogger<QiHttpServiceBase> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handle the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        protected async Task<T> HandleResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
                return await httpResponseMessage.Content.ReadAsAsync<T>();

            _logger.LogError($"Call to service returned {httpResponseMessage.StatusCode}");
            throw new RethrowApiException(httpResponseMessage);
        }

    }
}
