using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QI.WikiScraping.Api.Infrastructure.Utils.Validators;

namespace QI.WikiScraping.Api.Application.Services
{
    public class WikiScraperService : IWikiScraperService
    {
        private readonly ILogger<WikiScraperService> _logger;
        private readonly HttpClient _httpClient;


        public WikiScraperService(HttpClient httpClient, ILogger<WikiScraperService> logger)
        {
            QiArg.NotNull(logger, nameof(logger));
            QiArg.NotNull(httpClient, nameof(httpClient));

            _logger = logger;
            _httpClient = httpClient;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<dynamic> GetContentFromArticle(string url, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
