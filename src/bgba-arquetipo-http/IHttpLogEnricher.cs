using Microsoft.AspNetCore.Http;

namespace BgbaArquetipoHttp
{
    /// <summary>
    /// Interface for HTTP log enrichers that can add additional fields to HTTP logs
    /// </summary>
    public interface IHttpLogEnricher
    {
        /// <summary>
        /// Enrich the HTTP log entry with additional information
        /// </summary>
        /// <param name="httpLogEntry">The HTTP log entry to enrich</param>
        /// <param name="context">The HTTP context</param>
        void EnrichHttpLog(HttpLogEntry httpLogEntry, HttpContext context);

        /// <summary>
        /// Priority order for enricher execution (lower numbers execute first)
        /// </summary>
        int Priority { get; }
    }
}
