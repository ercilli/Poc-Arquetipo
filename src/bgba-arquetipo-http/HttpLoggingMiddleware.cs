using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BgbaArquetipoHttp.Middleware
{
    /// <summary>
    /// Middleware para interceptar requests y responses HTTP usando el nuevo patrón de contexto
    /// </summary>
    public class HttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpLogger _logger;

        public HttpLoggingMiddleware(RequestDelegate next, IHttpLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Crear contexto base desde HttpContext con logger name del middleware
            var logContext = HttpLogContextBuilder.FromHttpContext(context)
                .WithLoggerName(nameof(HttpLoggingMiddleware)); // Usar el nombre del middleware

            // Log incoming request usando el nuevo patrón
            _logger.Log(logContext.AsRequest());

            try
            {
                // Continue with the request pipeline
                await _next(context);

                stopwatch.Stop();

                // Log successful response con tiempo de respuesta
                _logger.Log(logContext
                    .AsResponse(context.Response.StatusCode)
                    .WithResponseTime(stopwatch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Log error response con detalles del error
                _logger.Log(logContext
                    .AsResponse(500, $"Error: {ex.Message}")
                    .WithResponseTime(stopwatch.ElapsedMilliseconds)
                    .WithProperty("ExceptionType", ex.GetType().Name)
                    .WithProperty("StackTrace", ex.StackTrace ?? ""));
                throw;
            }
        }
    }
}
