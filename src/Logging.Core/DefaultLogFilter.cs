namespace Logging.Core
{
    /// <summary>
    /// Default log filter implementation that respects LoggingConfiguration
    /// </summary>
    public class DefaultLogFilter : ILogFilter
    {
        private readonly LoggingConfiguration _configuration;

        public DefaultLogFilter(LoggingConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public bool ShouldLog(LogEntry logEntry, string? loggerName)
        {
            // Check minimum log level
            if (logEntry.Level < _configuration.MinimumLevel)
                return false;

            // If no logger name provided, allow the log
            if (string.IsNullOrEmpty(loggerName))
                return true;

            // If only arquetipo logs are enabled, check for arquetipo prefix
            if (_configuration.OnlyArquetipoLogs)
            {
                return loggerName.StartsWith(_configuration.ArquetipoPrefix, StringComparison.OrdinalIgnoreCase);
            }

            // Check explicit inclusions first (these override exclusions)
            if (_configuration.IncludedLoggerPrefixes.Any())
            {
                foreach (var includedPrefix in _configuration.IncludedLoggerPrefixes)
                {
                    if (loggerName.StartsWith(includedPrefix, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            // If system logs are suppressed, check exclusion list
            if (_configuration.SuppressSystemLogs)
            {
                foreach (var excludedPrefix in _configuration.ExcludedLoggerPrefixes)
                {
                    if (loggerName.StartsWith(excludedPrefix, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            // Default: allow the log
            return true;
        }
    }
}
