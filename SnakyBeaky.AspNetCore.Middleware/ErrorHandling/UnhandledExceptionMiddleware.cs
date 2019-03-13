using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SnakyBeaky.AspNetCore.Middleware.ErrorHandling
{
    /// <summary>
    /// Catches unhandled exceptions in the HTTP request pipeline.
    /// </summary>
    /// <remarks>
    /// Avoids leaking unwanted information to clients by returning an empty 500 InternalServerError.
    /// </remarks>
    public class UnhandledExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<UnhandledExceptionMiddleware> _logger;

        public UnhandledExceptionMiddleware(ILogger<UnhandledExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                string message = string.Join(' ',
                    "Unhandled",
                    ex.GetType().Name,
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.Connection.RemoteIpAddress.ToString(),
                    context.Connection.RemotePort.ToString()
                );

                _logger.LogError(message);

                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(string.Empty);
            }
        }
    }
}
