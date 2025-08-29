namespace Logging.Core
{
    /// <summary>
    /// Configuration options for logging behavior
    /// </summary>
    public class LoggingConfiguration
    {
        /// <summary>
        /// Whether to format JSON output with indentation
        /// </summary>
        public bool WriteIndented { get; set; } = false;

        /// <summary>
        /// Whether to include timestamp in logs
        /// </summary>
        public bool IncludeTimestamp { get; set; } = true;

        /// <summary>
        /// Minimum log level to output
        /// </summary>
        public LogLevel MinimumLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// Whether to suppress Microsoft and third-party logs
        /// </summary>
        public bool SuppressSystemLogs { get; set; } = true;

        /// <summary>
        /// List of logger name prefixes to exclude (e.g., "Microsoft", "System")
        /// </summary>
        public List<string> ExcludedLoggerPrefixes { get; set; } = new()
        {
            "Microsoft",
            "System",
            "Microsoft.Extensions",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore",
            "Microsoft.Hosting"
        };

        /// <summary>
        /// List of logger name prefixes to explicitly include (overrides exclusions)
        /// </summary>
        public List<string> IncludedLoggerPrefixes { get; set; } = new();

        /// <summary>
        /// Whether to include only logs from custom arquetipo components
        /// </summary>
        public bool OnlyArquetipoLogs { get; set; } = false;

        /// <summary>
        /// Prefix that identifies arquetipo components
        /// </summary>
        public string ArquetipoPrefix { get; set; } = "Bgba";

        /// <summary>
        /// Whether to exclude null properties from JSON output to save resources
        /// </summary>
        public bool ExcludeNullProperties { get; set; } = true;

        /// <summary>
        /// Whether to exclude empty string properties from JSON output
        /// </summary>
        public bool ExcludeEmptyStrings { get; set; } = false;

        /// <summary>
        /// Minimum string length to include in logs (0 = include all)
        /// Useful for excluding very short, non-meaningful values
        /// </summary>
        public int MinimumStringLength { get; set; } = 0;

        /// <summary>
        /// List of property names to always exclude from logs regardless of value
        /// Useful for sensitive or always-irrelevant fields
        /// </summary>
        public List<string> ExcludedPropertyNames { get; set; } = new();

        /// <summary>
        /// Whether to optimize output for minimal size (combines multiple optimizations)
        /// - Excludes null properties
        /// - Excludes empty strings
        /// - Uses minimal property names
        /// - Disables indentation
        /// </summary>
        public bool MinimalOutput { get; set; } = false;
    }
}
