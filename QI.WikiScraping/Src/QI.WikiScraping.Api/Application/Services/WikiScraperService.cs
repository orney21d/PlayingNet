using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using QI.WikiScraping.Api.Application.Exceptions;
using QI.WikiScraping.Api.Infrastructure.Utils.Validators;

namespace QI.WikiScraping.Api.Application.Services
{
    public class WikiScraperService : QiHttpServiceBase, IWikiScraperService
    {
        private readonly HttpClient _httpClient;


        public WikiScraperService(HttpClient httpClient,
            ILogger<WikiScraperService> logger)
            : base(logger)
        {

            QiArg.NotNull(httpClient, nameof(httpClient));

            _httpClient = httpClient;

        }

        /// <summary>
        /// Get the content from Wiki's Article and return the different words from it
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> GetContentFromArticle(string url, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Ini-- {nameof(GetContentFromArticle)}");

            if (string.IsNullOrWhiteSpace(url) || !url.ToLower().Contains("wikipedia.org", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new QualIdInvalidTestException($"Invalid URL's test exception. The test is meant to be made against Wikipedia's site.");
            }

            int result = 0;

            //Validate the Url from Wikipedia

            var response = await _httpClient.GetAsync(url, cancellationToken);



            //Getting the html content from Wiki
            string contentFromWiki = await HandleResponseAsString(response);


            //Get the text content from Body
            contentFromWiki  = await GetBodyContentTagText(contentFromWiki);

            #region Cleaning process
            //Removing end of line, cr, tabs, etc...
            contentFromWiki = CleanTextContent(contentFromWiki);

            #endregion

            //Checking we have any content after the cleanning
            if (string.IsNullOrWhiteSpace(contentFromWiki))
            {
                return result; // nothing to do.
            }

            //Getting the number of different words in the Wiki's article
            result = GetDifferentWordsCountInText(contentFromWiki);


            _logger.LogInformation($"End-- {nameof(GetContentFromArticle)}");
            return result;
        }

        /// <summary>
        /// Return the number of Different words in a String
        /// </summary>
        /// <returns></returns>
        private int GetDifferentWordsCountInText(string wikiPlainTextArticle) {

            wikiPlainTextArticle = wikiPlainTextArticle.Trim().ToUpper();
            string[] wordsFromArticle = wikiPlainTextArticle.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> uniqueWords = new Dictionary<string, string>();

            foreach (var word in wordsFromArticle)
            {
                //If I havent added already the  word, then I do in order to count at the end the number of words
                if (!uniqueWords.ContainsKey(word))
                    uniqueWords.Add(word, null);

            }

            return uniqueWords.Keys.Count();

        }

        /// <summary>
        /// Get the body content to scrap from Wiki.
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        private async Task<string> GetBodyContentTagText(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                return null;
            }
            //htmlContent = WebUtility.HtmlDecode(htmlContent.Trim());

            var config = Configuration.Default;
            //Creating a anew Context for the given config
            IBrowsingContext context = BrowsingContext.New(config);

            //Creating a new Document
            IDocument document = await context.OpenAsync(req => req.Content(htmlContent));

            //Getting the content of the block in the dom relevant to the words to parse and count
            var bodyContentDiv = document.QuerySelector("body > #content > #bodyContent > #mw-content-text");

            foreach (var element in bodyContentDiv.QuerySelectorAll("style"))
            {
                element.Remove();
            }

            return bodyContentDiv.Text();


        }


        /// <summary>
        /// I wold define this method externally , but for testing purpose I'm defining it here.
        /// </summary>
        /// <returns></returns>
        private string CleanTextContent(string contentToClean)
        {

            if (string.IsNullOrWhiteSpace(contentToClean))
                return contentToClean;

            if (!string.IsNullOrWhiteSpace(contentToClean))
            {

                contentToClean = contentToClean.Replace("[edit]", " ");
            }

            if (!string.IsNullOrWhiteSpace(contentToClean))
            {

                contentToClean = contentToClean.Replace("\"", " ");
            }

            //Clean the Urls
            contentToClean = Regex.Replace(contentToClean, @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*", " ");
            
            //Cleaning the string
            contentToClean = Regex.Replace(contentToClean, @"([:.;,]+)|(\d)|([\t\r\n])|(\^+)|(\{+)|(\}+)|(\[+)|(\]+)|(\(+)|(\)+)|(\|+)|(\*+)|(\++)|(\?+)|(\\+)|(\/+)|(–+)|(-+)|(·+)|(’+)", " ");

            

            return contentToClean;

        }



    }
}
