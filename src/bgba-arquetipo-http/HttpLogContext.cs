using Logging.Core;

namespace BgbaArquetipoHttp
{
    /// <summary>
    /// Contexto que encapsula toda la información necesaria para logging HTTP
    /// </summary>
    public class HttpLogContext
    {
        public string TraceId { get; set; } = string.Empty;
        public string SpanId { get; set; } = string.Empty;
        public string HttpRequestPath { get; set; } = string.Empty;
        public string? OutgoingRequestPath { get; set; }
        public string Message { get; set; } = string.Empty;
        public LogLevel Level { get; set; } = LogLevel.Information;
        public LogType LogType { get; set; }
        
        // Campos adicionales que se pueden agregar en el futuro sin romper la API
        public string? UserId { get; set; }
        public string? CorrelationId { get; set; }
        public string? ClientIp { get; set; }
        public string? UserAgent { get; set; }
        public int? StatusCode { get; set; }
        public long? ResponseTimeMs { get; set; }
        public string? LoggerName { get; set; }
        public Dictionary<string, object>? AdditionalProperties { get; set; }

        public HttpLogContext()
        {
            AdditionalProperties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Agrega una propiedad adicional al contexto
        /// </summary>
        public HttpLogContext AddProperty(string key, object value)
        {
            AdditionalProperties ??= new Dictionary<string, object>();
            AdditionalProperties[key] = value;
            return this;
        }

        /// <summary>
        /// Crea un HttpLogEntry basado en este contexto
        /// </summary>
        public HttpLogEntry ToLogEntry()
        {
            var logEntry = new HttpLogEntry(Level, Message, LogType, TraceId, SpanId, HttpRequestPath)
            {
                OutgoingRequestPath = OutgoingRequestPath,
                LoggerName = LoggerName
            };

            // En el futuro, aquí se pueden mapear campos adicionales
            // Ejemplo: logEntry.UserId = UserId;

            return logEntry;
        }
    }
}
