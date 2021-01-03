using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QI.WikiScraping.Api.Infrastructure.Models;

namespace QI.WikiScraping.Api.Infrastructure.Filters
{
    /// <summary>
    /// Filter to validate the ModelState
    /// </summary>
    public class QiValidModelStateFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            //Getting All Validation Errors
            string[] validationErrors = context.ModelState
                .Keys
                .SelectMany(k => context.ModelState[k].Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();



            var error = QiHttpError.CreateHttpValidationError(
                status: HttpStatusCode.BadRequest,
                userMessage: new[] { "There are validation errors" },
                validationErrors: validationErrors);

            context.Result = new BadRequestObjectResult(error);
        }
    }
}
