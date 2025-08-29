using Logging.Core;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BgbaArquetipoHttp
{
    /// <summary>
    /// Builder fluido para crear HttpLogContext de manera sencilla
    /// </summary>
    public class HttpLogContextBuilder
    {
        private readonly HttpLogContext _context;

        public HttpLogContextBuilder()
        {
            _context = new HttpLogContext();
        }

        /// <summary>
        /// Crea un nuevo builder con valores por defecto desde el HttpContext
        /// </summary>
        public static HttpLogContextBuilder FromHttpContext(HttpContext httpContext, [CallerMemberName] string callerMember = "", [CallerFilePath] string callerFilePath = "")
        {
            var builder = new HttpLogContextBuilder();
            
            builder._context.TraceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
            builder._context.SpanId = Activity.Current?.SpanId.ToString() ?? Guid.NewGuid().ToString("N")[..16];
            builder._context.HttpRequestPath = httpContext.Request.Path.Value ?? "/";
            builder._context.ClientIp = httpContext.Connection.RemoteIpAddress?.ToString();
            builder._context.UserAgent = httpContext.Request.Headers["User-Agent"].ToString();
            
            // Solo configurar logger name automáticamente si se proporcionó el path
            if (!string.IsNullOrEmpty(callerFilePath))
            {
                builder._context.LoggerName = ExtractClassName(callerFilePath);
            }

            return builder;
        }

        /// <summary>
        /// Crea un nuevo builder con valores por defecto desde el HttpContext (sobrecarga sin caller attributes)
        /// </summary>
        public static HttpLogContextBuilder FromHttpContext(HttpContext httpContext)
        {
            var builder = new HttpLogContextBuilder();
            
            builder._context.TraceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
            builder._context.SpanId = Activity.Current?.SpanId.ToString() ?? Guid.NewGuid().ToString("N")[..16];
            builder._context.HttpRequestPath = httpContext.Request.Path.Value ?? "/";
            builder._context.ClientIp = httpContext.Connection.RemoteIpAddress?.ToString();
            builder._context.UserAgent = httpContext.Request.Headers["User-Agent"].ToString();

            return builder;
        }

        /// <summary>
        /// Crea un nuevo builder con valores mínimos
        /// </summary>
        public static HttpLogContextBuilder Create([CallerMemberName] string callerMember = "", [CallerFilePath] string callerFilePath = "")
        {
            var builder = new HttpLogContextBuilder();
            builder._context.LoggerName = ExtractClassName(callerFilePath);
            return builder;
        }

        /// <summary>
        /// Extrae el nombre de la clase del path del archivo
        /// </summary>
        private static string ExtractClassName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "Unknown";

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            return fileName;
        }

        public HttpLogContextBuilder WithTraceId(string traceId)
        {
            _context.TraceId = traceId;
            return this;
        }

        public HttpLogContextBuilder WithSpanId(string spanId)
        {
            _context.SpanId = spanId;
            return this;
        }

        public HttpLogContextBuilder WithHttpRequestPath(string path)
        {
            _context.HttpRequestPath = path;
            return this;
        }

        public HttpLogContextBuilder WithOutgoingRequestPath(string? path)
        {
            _context.OutgoingRequestPath = path;
            return this;
        }

        public HttpLogContextBuilder WithMessage(string message)
        {
            _context.Message = message;
            return this;
        }

        public HttpLogContextBuilder WithLevel(LogLevel level)
        {
            _context.Level = level;
            return this;
        }

        public HttpLogContextBuilder WithLogType(LogType logType)
        {
            _context.LogType = logType;
            return this;
        }

        public HttpLogContextBuilder WithUserId(string? userId)
        {
            _context.UserId = userId;
            return this;
        }

        public HttpLogContextBuilder WithCorrelationId(string? correlationId)
        {
            _context.CorrelationId = correlationId;
            return this;
        }

        public HttpLogContextBuilder WithClientIp(string? clientIp)
        {
            _context.ClientIp = clientIp;
            return this;
        }

        public HttpLogContextBuilder WithUserAgent(string? userAgent)
        {
            _context.UserAgent = userAgent;
            return this;
        }

        public HttpLogContextBuilder WithStatusCode(int statusCode)
        {
            _context.StatusCode = statusCode;
            return this;
        }

        public HttpLogContextBuilder WithResponseTime(long responseTimeMs)
        {
            _context.ResponseTimeMs = responseTimeMs;
            return this;
        }

        public HttpLogContextBuilder WithLoggerName(string? loggerName)
        {
            _context.LoggerName = loggerName;
            return this;
        }

        public HttpLogContextBuilder WithProperty(string key, object value)
        {
            _context.AddProperty(key, value);
            return this;
        }

        /// <summary>
        /// Configura automáticamente para logging de REQUEST
        /// </summary>
        public HttpLogContextBuilder AsRequest(string? customMessage = null)
        {
            _context.LogType = LogType.REQUEST;
            _context.Message = customMessage ?? $"Request Executed.";
            return this;
        }

        /// <summary>
        /// Configura automáticamente para logging de RESPONSE
        /// </summary>
        public HttpLogContextBuilder AsResponse(int? statusCode = null, string? customMessage = null)
        {
            _context.LogType = LogType.RESPONSE;
            if (statusCode.HasValue)
            {
                _context.StatusCode = statusCode;
            }
            _context.Message = customMessage ?? $"Response Executed.";
            return this;
        }

        /// <summary>
        /// Configura automáticamente para logging de OUTGOING_REQUEST
        /// </summary>
        public HttpLogContextBuilder AsOutgoingRequest(string outgoingPath, string? customMessage = null)
        {
            _context.LogType = LogType.OUTGOING_REQUEST;
            _context.OutgoingRequestPath = outgoingPath;
            _context.Message = customMessage ?? $"Outgoing request.";
            return this;
        }

        /// <summary>
        /// Configura automáticamente para logging de OUTGOING_RESPONSE
        /// </summary>
        public HttpLogContextBuilder AsOutgoingResponse(string outgoingPath, int? statusCode = null, string? customMessage = null)
        {
            _context.LogType = LogType.OUTGOING_RESPONSE;
            _context.OutgoingRequestPath = outgoingPath;
            if (statusCode.HasValue)
            {
                _context.StatusCode = statusCode;
            }
            _context.Message = customMessage ?? $"Outgoing response.";
            return this;
        }

        /// <summary>
        /// Construye el contexto final
        /// </summary>
        public HttpLogContext Build()
        {
            return _context;
        }

        /// <summary>
        /// Implicit conversion para facilitar el uso
        /// </summary>
        public static implicit operator HttpLogContext(HttpLogContextBuilder builder)
        {
            return builder.Build();
        }
    }
}
