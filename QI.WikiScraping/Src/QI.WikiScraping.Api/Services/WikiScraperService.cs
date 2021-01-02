using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QI.WikiScraping.Api.Services
{
    public class WikiScraperService : IWikiScraperService
    {
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
