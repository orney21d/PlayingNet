using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Add the HealthCheck middleware to PipeLine
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseQiHealthChecks(this IApplicationBuilder appBuilder)
        {

            HealthCheckOptions hchkOptions = new HealthCheckOptions()
            {

                ResponseWriter = async (context, report) =>
                {
                    var result = JsonSerializer.Serialize(
                       new
                       {

                           status = report.Status.ToString(),
                           errors = report.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                       }, new JsonSerializerOptions()
                       {
                           IgnoreNullValues = true,
                           WriteIndented = false
                       });
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            };

            //Setting the response status code for unabailable services as "503"
            hchkOptions.ResultStatusCodes[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable;

            appBuilder.UseHealthChecks("/health", hchkOptions);
            return appBuilder;

        }
    }
}
