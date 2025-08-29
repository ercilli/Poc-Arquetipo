namespace Logging.Core
{
    /// <summary>
    /// Interface for log entries that can provide logger name information
    /// </summary>
    public interface ILoggerNameProvider
    {
        /// <summary>
        /// Gets the name of the logger/class that created this log entry
        /// </summary>
        string? LoggerName { get; }
    }
}
