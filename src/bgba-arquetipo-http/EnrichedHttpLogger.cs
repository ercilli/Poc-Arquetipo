using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Logging.Core;
using BgbaArquetipoHttp;
using System.Runtime.CompilerServices;

namespace BgbaArquetipoHttp
{
    /// <summary>
    /// Enhanced logger wrapper that enriches standard Microsoft logs with HTTP context
    /// </summary>
    public class EnrichedHttpLogger<T> : ILogger<T>
    {
        private readonly IHttpLogger _httpLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EnrichedHttpLogger(IHttpLogger httpLogger, IHttpContextAccessor httpContextAccessor)
        {
            _httpLogger = httpLogger ?? throw new ArgumentNullException(nameof(httpLogger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => true;

        public void Log<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var mappedLevel = MapLogLevel(logLevel);

            // Create enriched log entry
            var logEntry = CreateEnrichedLogEntry(message, mappedLevel, exception, typeof(T).Name);
            _httpLogger.Log(logEntry);
        }

        private HttpLogEntry CreateEnrichedLogEntry(string message, Logging.Core.LogLevel level, Exception? exception, string loggerName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            if (httpContext != null)
            {
                // Use existing HttpLogContext creation with auto-enrichment
                var context = HttpLogContextBuilder.FromHttpContext(httpContext)
                    .WithMessage(message)
                    .WithLevel(level)
                    .WithLoggerName(loggerName)
                    .WithLogType(DetermineLogType(level, httpContext));

                // Add exception details if present
                if (exception != null)
                {
                    context.WithProperty("ExceptionType", exception.GetType().Name)
                           .WithProperty("ExceptionMessage", exception.Message);
                }

                return context.Build().ToLogEntry();
            }
            else
            {
                // Fallback for non-HTTP context
                return new HttpLogEntry(level, message, LogType.REQUEST)
                {
                    LoggerName = loggerName,
                    TraceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString(),
                    SpanId = System.Diagnostics.Activity.Current?.SpanId.ToString() ?? Guid.NewGuid().ToString("N")[..16]
                };
            }
        }

        private LogType DetermineLogType(Logging.Core.LogLevel level, HttpContext httpContext)
        {
            if (level == Logging.Core.LogLevel.Error || level == Logging.Core.LogLevel.Critical)
                return LogType.RESPONSE;

            return !httpContext.Response.HasStarted ? LogType.REQUEST : LogType.RESPONSE;
        }

        private Logging.Core.LogLevel MapLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return logLevel switch
            {
                Microsoft.Extensions.Logging.LogLevel.Trace => Logging.Core.LogLevel.Trace,
                Microsoft.Extensions.Logging.LogLevel.Debug => Logging.Core.LogLevel.Debug,
                Microsoft.Extensions.Logging.LogLevel.Information => Logging.Core.LogLevel.Information,
                Microsoft.Extensions.Logging.LogLevel.Warning => Logging.Core.LogLevel.Warning,
                Microsoft.Extensions.Logging.LogLevel.Error => Logging.Core.LogLevel.Error,
                Microsoft.Extensions.Logging.LogLevel.Critical => Logging.Core.LogLevel.Critical,
                _ => Logging.Core.LogLevel.Information
            };
        }
    }
}
