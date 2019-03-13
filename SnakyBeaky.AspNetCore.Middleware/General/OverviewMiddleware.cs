using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SnakyBeaky.AspNetCore.Middleware.General
{
    /// <summary>
    /// Logs basic HTTP request and response data and elapsed time.
    /// </summary>
    public class OverviewMiddleware : IMiddleware
    {
        private readonly ILogger<OverviewMiddleware> _logger;

        public OverviewMiddleware(ILogger<OverviewMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Stopwatch watch = Stopwatch.StartNew();

            await next(context);

            watch.Stop();

            string message = string.Join(' ',
                context.Request.Method,
                context.Request.Path.Value,
                context.Connection.RemoteIpAddress.ToString(),
                context.Connection.RemotePort.ToString(),
                context.Response.StatusCode.ToString(),
                ((HttpStatusCode) context.Response.StatusCode).ToString("G"),
                watch.ElapsedMilliseconds + "ms"
            );

            _logger.LogInformation(message);
        }
    }
}
