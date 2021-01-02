using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using QI.WikiScraping.Api.Infrastructure.ActionResults;
using Microsoft.Extensions.Hosting;

namespace QI.WikiScraping.Api.Infrastructure.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);

            if (context.Exception.GetType() == typeof(RethrowApiException))
            {
                var exception = (RethrowApiException)context.Exception;
                if (exception.HttpResponseMessage != null)
                {
                    context.Result = new HttpResponseMessageResult(exception.HttpResponseMessage);
                }
                else
                {
                    if (exception.HttpStatus.Equals(HttpStatusCode.BadRequest))
                    {
                        context.Result = new BadRequestObjectResult(exception.ValidationProblemDetails);
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else if (exception.HttpStatus.Equals(HttpStatusCode.UnprocessableEntity))
                    {
                        context.Result = new UnprocessableEntityObjectResult(exception.ValidationProblemDetails);
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    }
                }


            }
            else
            {
                var json = new JsonErrorResponse
                {
                    Messages = new[] { "An error occur. Please call us." }
                };

                if (!env.IsProduction())
                {
                    json.DeveloperMessage = context.Exception;
                }

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.ExceptionHandled = true;

        }

        /// <summary>
        /// Handle a generic error message
        /// </summary>
        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMessage { get; set; }
        }
    }
}
