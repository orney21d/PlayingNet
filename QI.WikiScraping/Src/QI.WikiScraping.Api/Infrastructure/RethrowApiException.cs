using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace QI.WikiScraping.Api.Infrastructure
{
    /// <summary>
    /// To handle exceptions coming from external WebApi calls 
    /// </summary>
    public class RethrowApiException : Exception
    {
        public HttpStatusCode HttpStatus { get; }
        public ValidationProblemDetails ValidationProblemDetails { get; }
        public HttpResponseMessage HttpResponseMessage { get; }

        public RethrowApiException(HttpResponseMessage httpResponseMessage)
        {
            HttpResponseMessage = httpResponseMessage;
        }

        public RethrowApiException(HttpStatusCode httpStatus, ValidationProblemDetails validationProblemDetails)
        {
            HttpStatus = httpStatus;
            ValidationProblemDetails = validationProblemDetails;
        }

        public RethrowApiException(HttpStatusCode httpStatus, IDictionary<string, string[]> errors)
        {
            HttpStatus = httpStatus;
            ValidationProblemDetails = new ValidationProblemDetails(errors);
        }

        public RethrowApiException(HttpStatusCode httpStatus, IDictionary<string, string[]> errors, IDictionary<string, string> errorKeyReplacements)
        {
            HttpStatus = httpStatus;
            foreach (var replacement in errorKeyReplacements)
            {
                var error = errors.FirstOrDefault(e => e.Key.Split('.').Contains(replacement.Key));
                if (error.Key != null)
                {
                    errors.Remove(error.Key);
                    errors.Add(error.Key.Replace(replacement.Key, replacement.Value), error.Value);
                }
            }
            ValidationProblemDetails = new ValidationProblemDetails(errors);
        }
    }
}
