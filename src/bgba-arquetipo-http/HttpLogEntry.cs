using Logging.Core;
using System;
using System.Collections.Generic;

namespace BgbaArquetipoHttp
{
    /// <summary>
    /// Extended log entry for HTTP-related logging
    /// </summary>
    public class HttpLogEntry : LogEntry, ILoggerNameProvider
    {
        public LogType LogType { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public string SpanId { get; set; } = string.Empty;
        public string HttpRequestPath { get; set; } = string.Empty;
        public string? OutgoingRequestPath { get; set; }
        public string? LoggerName { get; set; }

        // Canal-specific properties that can be set by enrichers
        public string? CanalId { get; set; }
        public string? CanalType { get; set; }
        public string? SessionId { get; set; }
        public string? OperationType { get; set; }
        public string? RemoteIp { get; set; }
        public string? UserAgent { get; set; }

        public HttpLogEntry() : base()
        {
        }

        public HttpLogEntry(LogLevel level, string message, LogType logType) : base(level, message)
        {
            LogType = logType;
        }

        public HttpLogEntry(LogLevel level, string message, LogType logType, string traceId, string spanId, string httpRequestPath)
            : base(level, message)
        {
            LogType = logType;
            TraceId = traceId;
            SpanId = spanId;
            HttpRequestPath = httpRequestPath;
        }
    }
}