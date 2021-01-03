using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace QI.WikiScraping.Api.Infrastructure.Models
{
    /// <summary>
    /// Class to manage in a centralized way the errors returned to any request
    /// </summary>
    public class QiHttpError : IQiHttpError
    {
        /// <summary>
        /// Status code
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Status code as string
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Friendly User messages
        /// </summary>
        public string[] UserMessage { get; set; }

        /// <summary>
        /// If is in Dev mode contain a message as is for Developer
        /// </summary>
        public string DeveloperMessage { get; set; }

        /// <summary>
        /// Errors of validations Ex: bad userName, etc...
        /// </summary>
        public string[] ValidationErrors { get; set; }

        /// <summary>
        /// Create a representation object of validation errors
        /// </summary>
        /// <param name="status"></param>
        /// <param name="userMessage"></param>
        /// <param name="validationErrors"></param>
        /// <returns></returns>
        public static IQiHttpError CreateHttpValidationError(
            HttpStatusCode status,
            string[] userMessage,
            string[] validationErrors)
        {
            var httpError = CreateDefaultHttpError(status, userMessage);

            httpError.ValidationErrors = validationErrors;

            return httpError;
        }

        /// <summary>
        /// Create HttpError object with values
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="status"></param>
        /// <param name="code"></param>
        /// <param name="userMessage"></param>
        /// <param name="developerMessage"></param>
        /// <returns></returns>
        public static IQiHttpError Create(
            IWebHostEnvironment environment,
            HttpStatusCode status,
            string code,
            string[] userMessage,
            string developerMessage)
        {
            var httpError = CreateDefaultHttpError(status, userMessage);

            httpError.Code = code;

            if (environment.IsDevelopment())
            {
                httpError.DeveloperMessage = developerMessage;
            }

            return httpError;
        }

        /// <summary>
        /// Create the HttpError default object
        /// </summary>
        /// <param name="status"></param>
        /// <param name="userMessage"></param>
        /// <returns></returns>
        private static IQiHttpError CreateDefaultHttpError(
            HttpStatusCode status,
            string[] userMessage)
        {
            var httpError = new QiHttpError
            {
                Status = (int)status,
                UserMessage = userMessage
            };

            return httpError;
        }
    }
}
