using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logging.Core
{
    /// <summary>
    /// Console log writer implementation with configurable formatting, filtering, and optimization
    /// </summary>
    public class ConsoleLogWriter : ILogWriter
    {
        private readonly LoggingConfiguration _config;
        private readonly ILogFilter? _logFilter;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConsoleLogWriter(bool writeIndented = false, ILogFilter? logFilter = null, LoggingConfiguration? config = null)
        {
            _config = config ?? new LoggingConfiguration { WriteIndented = writeIndented };
            _logFilter = logFilter;

            // Use optimized JSON serialization based on configuration
            _jsonOptions = OptimizedJsonSerializer.CreateOptions(_config);
        }

        public void Write(LogEntry logEntry)
        {
            // Extract logger name if available
            string? loggerName = null;

            // Try to get logger name from ILoggerNameProvider interface
            if (logEntry is ILoggerNameProvider loggerNameProvider)
            {
                loggerName = loggerNameProvider.LoggerName;
            }

            // Apply filter if configured
            if (_logFilter != null && !_logFilter.ShouldLog(logEntry, loggerName))
            {
                return; // Skip this log entry
            }

            string json;

            // 🔧 FIX: Only use optimization for truly minimal output, preserve HTTP/Canal context by default
            if (_config.MinimalOutput || _config.ExcludedPropertyNames.Count > 0)
            {
                // Use optimization only when explicitly requested or for excluded properties
                var optimizedProperties = LogEntryOptimizer.OptimizeLogEntry(logEntry, _config);
                json = JsonSerializer.Serialize(optimizedProperties, _jsonOptions);
            }
            else
            {
                // Standard serialization with JsonIgnoreCondition.WhenWritingNull for null optimization
                json = JsonSerializer.Serialize(logEntry, logEntry.GetType(), _jsonOptions);
            }

            Console.WriteLine(json);
        }
    }
}
