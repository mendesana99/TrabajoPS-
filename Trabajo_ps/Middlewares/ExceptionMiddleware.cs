using System.Net;
using System.Text.Json;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Trabajo_ps.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ha ocurrido un error inesperado.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception is DomainException domainException)
            {
                context.Response.StatusCode = domainException.StatusCode;
                var domainResult = JsonSerializer.Serialize(new { error = domainException.Message });
                return context.Response.WriteAsync(domainResult);
            }

            // Excepción no controlada
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var result = JsonSerializer.Serialize(new { error = "Ocurrió un error interno del servidor." });
            return context.Response.WriteAsync(result);
        }
    }
}
