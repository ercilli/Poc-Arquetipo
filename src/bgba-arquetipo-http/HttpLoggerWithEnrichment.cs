using Logging.Core;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace BgbaArquetipoHttp
{
    /// <summary>
    /// HTTP logger that applies enrichers before logging
    /// </summary>
    public class HttpLoggerWithEnrichment : HttpLogger
    {
        private readonly IEnumerable<IHttpLogEnricher> _enrichers;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpLoggerWithEnrichment(
            ILogWriter logWriter,
            LoggingConfiguration config,
            IEnumerable<IHttpLogEnricher> enrichers,
            IHttpContextAccessor httpContextAccessor)
            : base()
        {
            // Add the writer to the base logger
            AddWriter(logWriter);

            _enrichers = enrichers.OrderBy(e => e.Priority);
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Log with enrichment applied
        /// </summary>
        public override void Log(HttpLogContext context)
        {
            // Convert to log entry
            var logEntry = context.ToLogEntry();

            // Apply enrichers if we have HTTP context available
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                foreach (var enricher in _enrichers)
                {
                    try
                    {
                        enricher.EnrichHttpLog(logEntry, httpContext);
                    }
                    catch (System.Exception ex)
                    {
                        // Log enricher errors but don't fail the request
                        System.Diagnostics.Debug.WriteLine($"Error in enricher {enricher.GetType().Name}: {ex.Message}");
                    }
                }
            }

            // Log the enriched entry
            Log(logEntry);
        }
    }
}
