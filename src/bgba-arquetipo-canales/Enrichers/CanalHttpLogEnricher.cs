using BgbaArquetipoHttp;
using Microsoft.AspNetCore.Http;

namespace BgbaArquetipoCanales.Enrichers
{
    /// <summary>
    /// HTTP log enricher that adds canal-specific information to HTTP logs
    /// </summary>
    public class CanalHttpLogEnricher : IHttpLogEnricher
    {
        public int Priority => 100; // Execute after basic HTTP enrichers

        public void EnrichHttpLog(HttpLogEntry httpLogEntry, HttpContext context)
        {
            // Add canal-specific information from headers or context

            // Canal ID from header
            if (context.Request.Headers.TryGetValue("X-Canal-Id", out var canalId))
            {
                httpLogEntry.CanalId = canalId.ToString();
            }

            // Canal type from header
            if (context.Request.Headers.TryGetValue("X-Canal-Type", out var canalType))
            {
                httpLogEntry.CanalType = canalType.ToString();
            }

            // User agent for canal identification
            if (context.Request.Headers.TryGetValue("User-Agent", out var userAgent))
            {
                httpLogEntry.UserAgent = userAgent.ToString();
            }

            // IP address for audit
            var remoteIp = context.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(remoteIp))
            {
                httpLogEntry.RemoteIp = remoteIp;
            }

            // Operation type based on HTTP method and path
            var operationType = DetermineOperationType(context.Request.Method, context.Request.Path.Value ?? "");
            httpLogEntry.OperationType = operationType;

            // Session ID for tracking
            if (context.Request.Headers.TryGetValue("X-Session-Id", out var sessionId))
            {
                httpLogEntry.SessionId = sessionId.ToString();
            }
        }

        private static string DetermineOperationType(string method, string path)
        {
            return method.ToUpper() switch
            {
                "GET" when path?.Contains("/api/") == true => "API_QUERY",
                "POST" when path?.Contains("/api/") == true => "API_COMMAND",
                "PUT" when path?.Contains("/api/") == true => "API_UPDATE",
                "DELETE" when path?.Contains("/api/") == true => "API_DELETE",
                "GET" => "WEB_REQUEST",
                "POST" => "WEB_SUBMIT",
                _ => "UNKNOWN"
            };
        }
    }
}
