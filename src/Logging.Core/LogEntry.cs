using System;

namespace Logging.Core
{
    /// <summary>
    /// Base log entry with core logging fields
    /// </summary>
    public class LogEntry
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;

        public LogEntry()
        {
        }

        public LogEntry(LogLevel level, string message)
        {
            Level = level;
            Message = message;
        }
    }
}
