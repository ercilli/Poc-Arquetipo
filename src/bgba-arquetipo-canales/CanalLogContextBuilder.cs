using Logging.Core;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace BgbaArquetipoCanales
{
    /// <summary>
    /// Builder for creating CanalLogContext with fluent API
    /// </summary>
    public class CanalLogContextBuilder
    {
        private readonly CanalLogContext _context = new();

        /// <summary>
        /// Sets the identity ID
        /// </summary>
        public CanalLogContextBuilder WithIdIdentidad(string? idIdentidad)
        {
            _context.IdIdentidad = idIdentidad;
            return this;
        }

        /// <summary>
        /// Sets the canal ID
        /// </summary>
        public CanalLogContextBuilder WithCanalId(string? canalId)
        {
            _context.CanalId = canalId;
            return this;
        }

        /// <summary>
        /// Sets the operation type
        /// </summary>
        public CanalLogContextBuilder WithOperationType(CanalOperationType operationType)
        {
            _context.OperationType = operationType;
            return this;
        }

        /// <summary>
        /// Sets the correlation ID
        /// </summary>
        public CanalLogContextBuilder WithCorrelationId(string? correlationId)
        {
            _context.CorrelationId = correlationId;
            return this;
        }

        /// <summary>
        /// Sets the logger name
        /// </summary>
        public CanalLogContextBuilder WithLoggerName(string? loggerName)
        {
            _context.LoggerName = loggerName;
            return this;
        }

        /// <summary>
        /// Auto-detects logger name from caller file path
        /// </summary>
        public CanalLogContextBuilder WithAutoLoggerName([CallerFilePath] string callerFilePath = "")
        {
            if (!string.IsNullOrEmpty(callerFilePath))
            {
                var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
                _context.LoggerName = fileName;
            }
            return this;
        }

        /// <summary>
        /// Sets the log message
        /// </summary>
        public CanalLogContextBuilder WithMessage(string message)
        {
            _context.Message = message;
            return this;
        }

        /// <summary>
        /// Sets the log level
        /// </summary>
        public CanalLogContextBuilder WithLogLevel(LogLevel logLevel)
        {
            _context.LogLevel = logLevel;
            return this;
        }

        /// <summary>
        /// Auto-populates from HttpContext
        /// </summary>
        public CanalLogContextBuilder FromHttpContext(HttpContext httpContext, [CallerFilePath] string callerFilePath = "")
        {
            var context = CanalLogContext.FromHttpContext(httpContext, callerFilePath);
            
            _context.IdIdentidad = context.IdIdentidad;
            _context.CanalId = context.CanalId;
            _context.CorrelationId = context.CorrelationId;
            _context.LoggerName = context.LoggerName;
            
            return this;
        }

        /// <summary>
        /// Auto-populates from HttpContext without caller attributes
        /// </summary>
        public CanalLogContextBuilder FromHttpContext(HttpContext httpContext)
        {
            var context = CanalLogContext.FromHttpContext(httpContext);
            
            _context.IdIdentidad = context.IdIdentidad;
            _context.CanalId = context.CanalId;
            _context.CorrelationId = context.CorrelationId;
            
            return this;
        }

        /// <summary>
        /// Builds the final context
        /// </summary>
        public CanalLogContext Build() => _context;

        /// <summary>
        /// Builds and converts to CanalLogEntry
        /// </summary>
        public CanalLogEntry BuildLogEntry() => _context.ToLogEntry();
    }
}
