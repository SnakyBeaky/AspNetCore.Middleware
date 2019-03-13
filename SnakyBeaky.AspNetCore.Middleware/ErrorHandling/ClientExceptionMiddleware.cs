using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SnakyBeaky.AspNetCore.Middleware.ErrorHandling
{
    /// <summary>
    /// Catches intentionally thrown exceptions in the HTTP request pipeline.
    /// </summary>
    /// <remarks>
    /// Returns the exception message to the client in a 403 Forbidden.
    /// </remarks>
    public class ClientExceptionMiddleware<T> : IMiddleware
        where T : Exception
    {
        private readonly ILogger<ClientExceptionMiddleware<T>> _logger;

        public ClientExceptionMiddleware(ILogger<ClientExceptionMiddleware<T>> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (T ex)
            {
                string message = string.Join(' ',
                    ex.GetType().Name,
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.Connection.RemoteIpAddress.ToString(),
                    context.Connection.RemotePort.ToString()
                );

                _logger.LogInformation(message);

                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = (int) HttpStatusCode.Forbidden;

                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}
