using Logging.Core;

namespace BgbaArquetipoCanales
{
    /// <summary>
    /// Logger implementation for canal-specific logging
    /// </summary>
    public class CanalLogger
    {
        private readonly ILogger _baseLogger;

        public CanalLogger(ILogger baseLogger)
        {
            _baseLogger = baseLogger ?? throw new ArgumentNullException(nameof(baseLogger));
        }

        /// <summary>
        /// Logs using CanalLogEntry
        /// </summary>
        public void Log(CanalLogEntry logEntry)
        {
            _baseLogger.Log(logEntry);
        }

        /// <summary>
        /// Logs using CanalLogContext
        /// </summary>
        public void Log(CanalLogContext context)
        {
            var logEntry = context.ToLogEntry();
            _baseLogger.Log(logEntry);
        }

        /// <summary>
        /// Convenience method for quick logging
        /// </summary>
        public void Log(LogLevel level, string message, CanalOperationType operationType, string? idIdentidad = null, string? canalId = null)
        {
            var logEntry = new CanalLogEntry(level, message, operationType, idIdentidad, canalId);
            _baseLogger.Log(logEntry);
        }

        /// <summary>
        /// Creates a new context builder
        /// </summary>
        public CanalLogContextBuilder CreateContext()
        {
            return new CanalLogContextBuilder();
        }
    }
}
