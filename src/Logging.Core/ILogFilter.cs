namespace Logging.Core
{
    /// <summary>
    /// Interface for filtering log entries
    /// </summary>
    public interface ILogFilter
    {
        /// <summary>
        /// Determines if a log entry should be processed
        /// </summary>
        /// <param name="logEntry">The log entry to evaluate</param>
        /// <param name="loggerName">Name of the logger (class) that created the log</param>
        /// <returns>True if the log should be processed, false if it should be filtered out</returns>
        bool ShouldLog(LogEntry logEntry, string? loggerName);
    }
}
