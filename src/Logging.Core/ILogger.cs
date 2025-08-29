using System;
using System.Collections.Generic;

namespace Logging.Core
{
    /// <summary>
    /// Interface for logging implementations
    /// </summary>
    public interface ILogger
    {
        void Log(LogEntry logEntry);
        void Log(LogLevel level, string message);
        void Trace(string message);
        void Debug(string message);
        void Information(string message);
        void Warning(string message);
        void Error(string message);
        void Critical(string message);
    }

    /// <summary>
    /// Basic logger implementation
    /// </summary>
    public class Logger : ILogger
    {
        private readonly List<ILogWriter> _writers;

        public Logger()
        {
            _writers = new List<ILogWriter>();
        }

        public void AddWriter(ILogWriter writer)
        {
            _writers.Add(writer);
        }

        public void Log(LogEntry logEntry)
        {
            foreach (var writer in _writers)
            {
                writer.Write(logEntry);
            }
        }

        public void Log(LogLevel level, string message)
        {
            Log(new LogEntry(level, message));
        }

        public void Trace(string message) => Log(LogLevel.Trace, message);
        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Information(string message) => Log(LogLevel.Information, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message) => Log(LogLevel.Error, message);
        public void Critical(string message) => Log(LogLevel.Critical, message);
    }
}
