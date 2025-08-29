using Logging.Core;

namespace BgbaArquetipoHttp
{
    /// <summary>
    /// HTTP-specific logger implementation
    /// </summary>
    public interface IHttpLogger : ILogger
    {
        // Nuevo método principal que usa contexto
        void Log(HttpLogContext context);
        
        // Métodos de conveniencia para mantener compatibilidad (marcados como obsoletos)
        [Obsolete("Use Log(HttpLogContext) instead. This method will be removed in future versions.")]
        void LogRequest(string traceId, string spanId, string httpRequestPath, string message = "HTTP Request");
        
        [Obsolete("Use Log(HttpLogContext) instead. This method will be removed in future versions.")]
        void LogResponse(string traceId, string spanId, string httpRequestPath, string message = "HTTP Response");
        
        [Obsolete("Use Log(HttpLogContext) instead. This method will be removed in future versions.")]
        void LogOutgoingRequest(string traceId, string spanId, string httpRequestPath, string outgoingRequestPath, string message = "Outgoing HTTP Request");
        
        [Obsolete("Use Log(HttpLogContext) instead. This method will be removed in future versions.")]
        void LogOutgoingResponse(string traceId, string spanId, string httpRequestPath, string outgoingRequestPath, string message = "Outgoing HTTP Response");
    }

    /// <summary>
    /// HTTP logger implementation
    /// </summary>
    public class HttpLogger : Logger, IHttpLogger
    {
        /// <summary>
        /// Método principal para logging HTTP usando contexto
        /// </summary>
        public void Log(HttpLogContext context)
        {
            var logEntry = context.ToLogEntry();
            Log(logEntry);
        }

        /// <summary>
        /// Método de conveniencia para crear contexto y loggear request
        /// </summary>
        public void LogRequest(HttpLogContext context)
        {
            context.LogType = LogType.REQUEST;
            if (string.IsNullOrEmpty(context.Message))
            {
                context.Message = $"Incoming request: {context.HttpRequestPath}";
            }
            Log(context);
        }

        /// <summary>
        /// Método de conveniencia para crear contexto y loggear response
        /// </summary>
        public void LogResponse(HttpLogContext context)
        {
            context.LogType = LogType.RESPONSE;
            if (string.IsNullOrEmpty(context.Message))
            {
                context.Message = $"Response: {context.StatusCode ?? 200} for {context.HttpRequestPath}";
            }
            Log(context);
        }

        /// <summary>
        /// Método de conveniencia para crear contexto y loggear outgoing request
        /// </summary>
        public void LogOutgoingRequest(HttpLogContext context)
        {
            context.LogType = LogType.OUTGOING_REQUEST;
            if (string.IsNullOrEmpty(context.Message))
            {
                context.Message = $"Outgoing request: {context.OutgoingRequestPath}";
            }
            Log(context);
        }

        /// <summary>
        /// Método de conveniencia para crear contexto y loggear outgoing response
        /// </summary>
        public void LogOutgoingResponse(HttpLogContext context)
        {
            context.LogType = LogType.OUTGOING_RESPONSE;
            if (string.IsNullOrEmpty(context.Message))
            {
                context.Message = $"Outgoing response: {context.StatusCode ?? 200} from {context.OutgoingRequestPath}";
            }
            Log(context);
        }

        // Métodos legacy para mantener compatibilidad hacia atrás
        [Obsolete("Use Log(HttpLogContext) instead. This method will be removed in future versions.")]
        public void LogRequest(string traceId, string spanId, string httpRequestPath, string message = "HTTP Request")
        {
            var context = HttpLogContextBuilder.Create()
                .WithTraceId(traceId)
                .WithSpanId(spanId)
                .WithHttpRequestPath(httpRequestPath)
                .AsRequest(message)
                .Build();
            Log(context);
        }

        [Obsolete("Use Log(HttpLogContext) instead. This method will be removed in future versions.")]
        public void LogResponse(string traceId, string spanId, string httpRequestPath, string message = "HTTP Response")
        {
            var context = HttpLogContextBuilder.Create()
                .WithTraceId(traceId)
                .WithSpanId(spanId)
                .WithHttpRequestPath(httpRequestPath)
                .AsResponse(customMessage: message)
                .Build();
            Log(context);
        }

        [Obsolete("Use Log(HttpLogContext) instead. This method will be removed in future versions.")]
        public void LogOutgoingRequest(string traceId, string spanId, string httpRequestPath, string outgoingRequestPath, string message = "Outgoing HTTP Request")
        {
            var context = HttpLogContextBuilder.Create()
                .WithTraceId(traceId)
                .WithSpanId(spanId)
                .WithHttpRequestPath(httpRequestPath)
                .AsOutgoingRequest(outgoingRequestPath, message)
                .Build();
            Log(context);
        }

        [Obsolete("Use Log(HttpLogContext) instead. This method will be removed in future versions.")]
        public void LogOutgoingResponse(string traceId, string spanId, string httpRequestPath, string outgoingRequestPath, string message = "Outgoing HTTP Response")
        {
            var context = HttpLogContextBuilder.Create()
                .WithTraceId(traceId)
                .WithSpanId(spanId)
                .WithHttpRequestPath(httpRequestPath)
                .AsOutgoingResponse(outgoingRequestPath, customMessage: message)
                .Build();
            Log(context);
        }
    }
}
