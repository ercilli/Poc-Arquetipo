using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BgbaArquetipoHttp.Interceptors
{
    /// <summary>
    /// HTTP message handler para interceptar requests salientes usando el nuevo patrón de contexto
    /// </summary>
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly IHttpLogger _logger;

        public HttpLoggingHandler(IHttpLogger logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var outgoingRequestPath = request.RequestUri?.ToString() ?? "Unknown";
            
            // Crear contexto base
            var logContext = HttpLogContextBuilder.Create()
                .WithTraceId(Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString())
                .WithSpanId(Activity.Current?.SpanId.ToString() ?? Guid.NewGuid().ToString("N")[..16])
                .WithHttpRequestPath(GetCurrentRequestPath())
                .WithProperty("HttpMethod", request.Method.ToString())
                .WithProperty("RequestHeaders", request.Headers.ToString());

            // Log outgoing request
            _logger.Log(logContext.AsOutgoingRequest(outgoingRequestPath));

            try
            {
                // Send the request
                var response = await base.SendAsync(request, cancellationToken);
                
                stopwatch.Stop();

                // Log successful outgoing response
                _logger.Log(logContext
                    .AsOutgoingResponse(outgoingRequestPath, (int)response.StatusCode)
                    .WithResponseTime(stopwatch.ElapsedMilliseconds)
                    .WithProperty("ResponseHeaders", response.Headers.ToString())
                    .WithProperty("ContentType", response.Content.Headers.ContentType?.ToString() ?? ""));

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Log error outgoing response
                _logger.Log(logContext
                    .AsOutgoingResponse(outgoingRequestPath, 0, $"Error: {ex.Message}")
                    .WithResponseTime(stopwatch.ElapsedMilliseconds)
                    .WithProperty("ExceptionType", ex.GetType().Name)
                    .WithProperty("ErrorMessage", ex.Message));
                throw;
            }
        }

        private static string GetCurrentRequestPath()
        {
            // Try to get the current HTTP context path
            // This is a simplified implementation - in a real scenario you might want to use
            // IHttpContextAccessor or pass the current request path through headers
            return Activity.Current?.GetTagItem("http.url")?.ToString() ?? 
                   Activity.Current?.GetTagItem("http.target")?.ToString() ?? 
                   "/";
        }
    }
}
