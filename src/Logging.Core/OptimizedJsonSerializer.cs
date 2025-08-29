using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logging.Core
{
    /// <summary>
    /// Helper for optimized JSON serialization based on logging configuration
    /// </summary>
    public static class OptimizedJsonSerializer
    {
        /// <summary>
        /// Creates JsonSerializerOptions based on logging configuration
        /// </summary>
        public static JsonSerializerOptions CreateOptions(LoggingConfiguration config)
        {
            var options = new JsonSerializerOptions();

            // Basic formatting
            options.WriteIndented = config.MinimalOutput ? false : config.WriteIndented;

            // Property naming
            if (config.MinimalOutput)
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }
            else
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            }

            // Null handling
            if (config.ExcludeNullProperties || config.MinimalOutput)
            {
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }

            // Add custom converter for additional optimizations
            if (config.ExcludeEmptyStrings || config.MinimumStringLength > 0 || config.ExcludedPropertyNames.Count > 0)
            {
                options.Converters.Add(new OptimizedStringConverter(config));
            }

            return options;
        }

        /// <summary>
        /// Serializes an object using optimized settings
        /// </summary>
        public static string Serialize<T>(T obj, LoggingConfiguration config)
        {
            var options = CreateOptions(config);
            return JsonSerializer.Serialize(obj, options);
        }
    }

    /// <summary>
    /// Custom JSON converter that applies string optimizations
    /// </summary>
    public class OptimizedStringConverter : JsonConverter<string>
    {
        private readonly LoggingConfiguration _config;

        public OptimizedStringConverter(LoggingConfiguration config)
        {
            _config = config;
        }

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            // Check if we should exclude this string
            if (ShouldExcludeString(value))
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value);
        }

        private bool ShouldExcludeString(string? value)
        {
            if (value == null)
                return true;

            // Exclude empty strings if configured
            if (_config.ExcludeEmptyStrings && string.IsNullOrEmpty(value))
                return true;

            // Exclude strings below minimum length
            if (_config.MinimumStringLength > 0 && value.Length < _config.MinimumStringLength)
                return true;

            return false;
        }
    }

    /// <summary>
    /// Extension methods for applying property exclusions
    /// </summary>
    public static class LogEntryOptimizer
    {
        /// <summary>
        /// Creates an optimized version of a log entry based on configuration
        /// </summary>
        public static Dictionary<string, object?> OptimizeLogEntry<T>(T logEntry, LoggingConfiguration config) where T : LogEntry
        {
            var properties = new Dictionary<string, object?>();

            // 🔧 FIX: Use actual runtime type instead of generic constraint to get ALL properties
            var actualType = logEntry.GetType();
            var allProperties = actualType.GetProperties();

            foreach (var prop in allProperties)
            {
                var value = prop.GetValue(logEntry);
                var propertyName = GetOptimizedPropertyName(prop.Name, config);

                // Skip if property is in exclusion list
                if (config.ExcludedPropertyNames.Contains(prop.Name) ||
                    config.ExcludedPropertyNames.Contains(propertyName))
                    continue;

                // Apply value-based optimizations
                if (ShouldIncludeProperty(value, config))
                {
                    properties[propertyName] = value;
                }
            }

            return properties;
        }

        private static bool ShouldIncludeProperty(object? value, LoggingConfiguration config)
        {
            // Always exclude null if configured
            if (value == null && (config.ExcludeNullProperties || config.MinimalOutput))
                return false;

            // Handle string-specific rules
            if (value is string stringValue)
            {
                if (config.ExcludeEmptyStrings && string.IsNullOrEmpty(stringValue))
                    return false;

                if (config.MinimumStringLength > 0 && stringValue.Length < config.MinimumStringLength)
                    return false;
            }

            return true;
        }

        private static string GetOptimizedPropertyName(string originalName, LoggingConfiguration config)
        {
            if (config.MinimalOutput)
            {
                // Use minimal property names for space optimization
                return originalName switch
                {
                    "Timestamp" => "ts",
                    "Level" => "lvl",
                    "Message" => "msg",
                    "LoggerName" => "src",
                    "TraceId" => "tid",
                    "SpanId" => "sid",
                    "HttpRequestPath" => "path",
                    "LogType" => "type",
                    "StatusCode" => "status",
                    "ResponseTime" => "time",
                    "IdIdentidad" => "uid",
                    "CanalId" => "ch",
                    "CorrelationId" => "cid",
                    _ => originalName.ToLower()
                };
            }

            // Convert to snake_case for standard output
            return ToSnakeCase(originalName);
        }

        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (i > 0 && char.IsUpper(input[i]))
                    result += "_";
                result += char.ToLower(input[i]);
            }
            return result;
        }
    }
}
