using Microsoft.AspNetCore.Http;
using Logging.Core;
using System.Runtime.CompilerServices;

namespace BgbaArquetipoHttp
{
    /// <summary>
    /// Extension methods for simplified logging API
    /// </summary>
    public static class HttpLoggerExtensions
    {
        /// <summary>
        /// Logs an information message with automatic HTTP context enrichment
        /// </summary>
        public static void LogInformation(this IHttpLogger logger, string message, [CallerFilePath] string callerFilePath = "")
        {
            logger.LogWithContext(message, LogLevel.Information, LogType.REQUEST, callerFilePath);
        }

        /// <summary>
        /// Logs a warning message with automatic HTTP context enrichment
        /// </summary>
        public static void LogWarning(this IHttpLogger logger, string message, [CallerFilePath] string callerFilePath = "")
        {
            logger.LogWithContext(message, LogLevel.Warning, LogType.REQUEST, callerFilePath);
        }

        /// <summary>
        /// Logs an error message with automatic HTTP context enrichment
        /// </summary>
        public static void LogError(this IHttpLogger logger, string message, [CallerFilePath] string callerFilePath = "")
        {
            logger.LogWithContext(message, LogLevel.Error, LogType.RESPONSE, callerFilePath);
        }

        /// <summary>
        /// Logs a debug message with automatic HTTP context enrichment
        /// </summary>
        public static void LogDebug(this IHttpLogger logger, string message, [CallerFilePath] string callerFilePath = "")
        {
            logger.LogWithContext(message, LogLevel.Debug, LogType.REQUEST, callerFilePath);
        }

        /// <summary>
        /// Internal method that automatically enriches logs with HTTP context
        /// </summary>
        private static void LogWithContext(this IHttpLogger logger, string message, LogLevel level, LogType logType, string callerFilePath)
        {
            // Try to get HTTP context from current request
            var httpContextAccessor = HttpContextAccessorProvider.Current;
            var httpContext = httpContextAccessor?.HttpContext;

            HttpLogEntry logEntry;

            if (httpContext != null)
            {
                // Auto-enrich with HTTP context
                var context = HttpLogContextBuilder.FromHttpContext(httpContext)
                    .WithMessage(message)
                    .WithLevel(level)
                    .WithLogType(logType);

                // Auto-detect logger name from caller file path
                if (!string.IsNullOrEmpty(callerFilePath))
                {
                    var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
                    context.WithLoggerName(fileName);
                }

                logEntry = context.Build().ToLogEntry();
            }
            else
            {
                // Fallback when no HTTP context
                logEntry = new HttpLogEntry(level, message, logType)
                {
                    TraceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString(),
                    SpanId = System.Diagnostics.Activity.Current?.SpanId.ToString() ?? Guid.NewGuid().ToString("N")[..16],
                    LoggerName = !string.IsNullOrEmpty(callerFilePath) 
                        ? Path.GetFileNameWithoutExtension(callerFilePath) 
                        : "Unknown"
                };
            }

            logger.Log(logEntry);
        }
    }

    /// <summary>
    /// Static provider for HttpContextAccessor to avoid DI complexity
    /// </summary>
    public static class HttpContextAccessorProvider
    {
        public static IHttpContextAccessor? Current { get; set; }
    }
}
