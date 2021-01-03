using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QI.WikiScraping.Api.Application.Services;

namespace QI.WikiScraping.Api.Controllers.V1
{

    [ApiVersion("1.0")]
    [Route("api/V{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class WikiArticleController : ControllerBase
    {
        private readonly ILogger<WikiArticleController> _logger;
        private readonly IWikiScraperService _wikiScraperService;


        public WikiArticleController(ILogger<WikiArticleController> logger,
            IWikiScraperService wikiScraperService)
        {
            _logger = logger;
            _wikiScraperService = wikiScraperService;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Route(nameof(DifferentWords))]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public  async Task<ActionResult> DifferentWords(string wikiArticleUrl, CancellationToken cancellationToken)
        {
            return Ok( new { words= 5 });
        }
    }
}
