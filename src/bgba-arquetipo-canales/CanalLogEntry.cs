using Logging.Core;

namespace BgbaArquetipoCanales
{
    /// <summary>
    /// Extended log entry for Canales-related logging with identity tracking
    /// </summary>
    public class CanalLogEntry : LogEntry, ILoggerNameProvider
    {
        /// <summary>
        /// Identity ID extracted from request headers (id_identidad)
        /// </summary>
        public string? IdIdentidad { get; set; }
        
        /// <summary>
        /// Channel identifier
        /// </summary>
        public string? CanalId { get; set; }
        
        /// <summary>
        /// Type of canal operation (authentication, transaction, etc.)
        /// </summary>
        public CanalOperationType OperationType { get; set; }
        
        /// <summary>
        /// Request correlation ID for tracking across microservices
        /// </summary>
        public string? CorrelationId { get; set; }
        
        /// <summary>
        /// Logger name for identifying the source class
        /// </summary>
        public string? LoggerName { get; set; }

        public CanalLogEntry() : base()
        {
        }

        public CanalLogEntry(LogLevel level, string message, CanalOperationType operationType) : base(level, message)
        {
            OperationType = operationType;
        }

        public CanalLogEntry(LogLevel level, string message, CanalOperationType operationType, string? idIdentidad, string? canalId) 
            : base(level, message)
        {
            OperationType = operationType;
            IdIdentidad = idIdentidad;
            CanalId = canalId;
        }
    }
}
