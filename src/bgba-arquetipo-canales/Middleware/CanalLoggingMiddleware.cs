using Logging.Core;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace BgbaArquetipoCanales.Middleware
{
    /// <summary>
    /// Middleware for automatic canal logging on all requests
    /// </summary>
    public class CanalLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CanalLogger _logger;

        public CanalLoggingMiddleware(RequestDelegate next, CanalLogger logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request start
            _logger.Log(new CanalLogContextBuilder()
                .FromHttpContext(context)
                .WithLoggerName(nameof(CanalLoggingMiddleware))
                .WithMessage($"Canal request started: {context.Request.Method} {context.Request.Path}")
                .WithLogLevel(LogLevel.Information)
                .WithOperationType(CanalOperationType.Authentication)
                .Build());

            try
            {
                await _next(context);

                // Log successful completion
                _logger.Log(new CanalLogContextBuilder()
                    .FromHttpContext(context)
                    .WithLoggerName(nameof(CanalLoggingMiddleware))
                    .WithMessage($"Canal request completed successfully: {context.Response.StatusCode}")
                    .WithLogLevel(LogLevel.Information)
                    .WithOperationType(CanalOperationType.Transaction)
                    .Build());
            }
            catch (Exception ex)
            {
                // Log error
                _logger.Log(new CanalLogContextBuilder()
                    .FromHttpContext(context)
                    .WithLoggerName(nameof(CanalLoggingMiddleware))
                    .WithMessage($"Canal request failed: {ex.Message}")
                    .WithLogLevel(LogLevel.Error)
                    .WithOperationType(CanalOperationType.ErrorHandling)
                    .Build());

                throw;
            }
        }
    }
}
