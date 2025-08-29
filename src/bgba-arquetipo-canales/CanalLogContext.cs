using Logging.Core;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace BgbaArquetipoCanales
{
    /// <summary>
    /// Context object for canal logging with builder pattern support
    /// </summary>
    public class CanalLogContext
    {
        public string? IdIdentidad { get; set; }
        public string? CanalId { get; set; }
        public CanalOperationType OperationType { get; set; }
        public string? CorrelationId { get; set; }
        public string? LoggerName { get; set; }
        public string Message { get; set; } = string.Empty;
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Creates a CanalLogEntry from this context
        /// </summary>
        public CanalLogEntry ToLogEntry()
        {
            return new CanalLogEntry(LogLevel, Message, OperationType, IdIdentidad, CanalId)
            {
                CorrelationId = CorrelationId,
                LoggerName = LoggerName
            };
        }

        /// <summary>
        /// Creates context from HttpContext automatically extracting canal-specific headers
        /// </summary>
        public static CanalLogContext FromHttpContext(HttpContext httpContext, [CallerFilePath] string callerFilePath = "")
        {
            var context = new CanalLogContext();
            
            // Extract id_identidad from headers
            if (httpContext.Request.Headers.TryGetValue("id_identidad", out var idIdentidad))
            {
                context.IdIdentidad = idIdentidad.FirstOrDefault();
            }
            
            // Extract canal_id from headers
            if (httpContext.Request.Headers.TryGetValue("canal_id", out var canalId))
            {
                context.CanalId = canalId.FirstOrDefault();
            }
            
            // Extract correlation_id from headers (standard practice)
            if (httpContext.Request.Headers.TryGetValue("x-correlation-id", out var correlationId))
            {
                context.CorrelationId = correlationId.FirstOrDefault();
            }
            
            // Auto-detect logger name from caller file path
            if (!string.IsNullOrEmpty(callerFilePath))
            {
                var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
                context.LoggerName = fileName;
            }

            return context;
        }

        /// <summary>
        /// Creates context from HttpContext without caller attributes (for middleware use)
        /// </summary>
        public static CanalLogContext FromHttpContext(HttpContext httpContext)
        {
            var context = new CanalLogContext();
            
            // Extract id_identidad from headers
            if (httpContext.Request.Headers.TryGetValue("id_identidad", out var idIdentidad))
            {
                context.IdIdentidad = idIdentidad.FirstOrDefault();
            }
            
            // Extract canal_id from headers
            if (httpContext.Request.Headers.TryGetValue("canal_id", out var canalId))
            {
                context.CanalId = canalId.FirstOrDefault();
            }
            
            // Extract correlation_id from headers
            if (httpContext.Request.Headers.TryGetValue("x-correlation-id", out var correlationId))
            {
                context.CorrelationId = correlationId.FirstOrDefault();
            }

            return context;
        }
    }
}
