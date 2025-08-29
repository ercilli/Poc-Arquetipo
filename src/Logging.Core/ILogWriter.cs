namespace Logging.Core
{
    /// <summary>
    /// Interface for writing log entries to different outputs
    /// </summary>
    public interface ILogWriter
    {
        void Write(LogEntry logEntry);
    }
}
