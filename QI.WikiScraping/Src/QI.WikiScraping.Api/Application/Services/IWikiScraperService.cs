using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QI.WikiScraping.Api.Application.Services
{
    /// <summary>
    /// Define the contract for the scraping's service related to Wiki
    /// </summary>
    public interface IWikiScraperService
    {
        Task<dynamic> GetContentFromArticle(string url, CancellationToken cancellationToken);
    }
}
