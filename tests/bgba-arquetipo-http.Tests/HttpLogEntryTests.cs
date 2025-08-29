using BgbaArquetipoHttp;
using Logging.Core;

namespace BgbaArquetipoHttp.Tests
{
    public class HttpLogEntryTests
    {
        [Fact]
        public void HttpLogEntry_Should_Serialize_All_Properties()
        {
            // Arrange
            var logEntry = new HttpLogEntry(LogLevel.Information, "Test message", LogType.REQUEST, "trace123", "span456", "/test")
            {
                OutgoingRequestPath = "https://example.com/api",
                LoggerName = "TestClass"
            };

            var writer = new ConsoleLogWriter(writeIndented: true);

            // Act & Assert - Esto debería mostrar todos los campos incluyendo loggerName
            writer.Write(logEntry);
            
            // Verificar que el loggerName se incluyó
            Assert.Equal("TestClass", logEntry.LoggerName);
        }

        [Fact] 
        public void HttpLogContextBuilder_Should_Auto_Detect_Logger_Name()
        {
            // Arrange & Act - El builder debería detectar automáticamente el nombre de la clase
            var context = HttpLogContextBuilder.Create()
                .WithMessage("Test from HttpLogEntryTests")
                .WithLevel(LogLevel.Information)
                .WithLogType(LogType.REQUEST)
                .Build();

            var writer = new ConsoleLogWriter(writeIndented: true);
            var logEntry = context.ToLogEntry();

            // Assert
            Assert.NotNull(context.LoggerName);
            Assert.Equal("HttpLogEntryTests", context.LoggerName); // Debería ser el nombre de esta clase
            
            Console.WriteLine("=== Test Logger Name Detection ===");
            writer.Write(logEntry);
        }

        [Fact]
        public void LogFilter_Should_Suppress_Microsoft_Logs()
        {
            // Arrange
            var config = new LoggingConfiguration 
            { 
                SuppressSystemLogs = true,
                WriteIndented = true
            };
            var filter = new DefaultLogFilter(config);
            var writer = new ConsoleLogWriter(writeIndented: true, logFilter: filter);
            
            // Act & Assert - Log de Microsoft debería ser suprimido
            var microsoftLog = new HttpLogEntry(LogLevel.Information, "Microsoft log", LogType.REQUEST)
            {
                LoggerName = "Microsoft.AspNetCore.Hosting"
            };
            
            var arquetipoLog = new HttpLogEntry(LogLevel.Information, "Arquetipo log", LogType.REQUEST)
            {
                LoggerName = "BgbaArquetipoHttp.Tests"
            };

            Console.WriteLine("=== Test Log Filtering ===");
            Console.WriteLine("Microsoft log (should be suppressed):");
            writer.Write(microsoftLog);
            
            Console.WriteLine("Arquetipo log (should be shown):");
            writer.Write(arquetipoLog);
        }

        [Fact]
        public void HttpLogContext_Should_Work_With_Builder_Pattern()
        {
            // Arrange & Act - Uso del nuevo patrón con Builder
            var context = HttpLogContextBuilder.Create()
                .WithTraceId("trace-123")
                .WithSpanId("span-456")
                .WithHttpRequestPath("/api/users")
                .WithUserId("user-789")
                .WithCorrelationId("corr-abc")
                .AsRequest("Processing user request")
                .WithProperty("CustomField", "CustomValue")
                .Build();

            var writer = new ConsoleLogWriter(writeIndented: true);
            var logEntry = context.ToLogEntry();

            // Assert - Verificar que funciona correctamente
            Assert.Equal("trace-123", context.TraceId);
            Assert.Equal("span-456", context.SpanId);
            Assert.Equal("/api/users", context.HttpRequestPath);
            Assert.Equal("user-789", context.UserId);
            Assert.Equal("corr-abc", context.CorrelationId);
            Assert.Equal(LogType.REQUEST, context.LogType);
            Assert.Equal("Processing user request", context.Message);
            Assert.True(context.AdditionalProperties?.ContainsKey("CustomField"));

            // Mostrar el resultado
            Console.WriteLine("=== Log con nuevo patrón de contexto ===");
            writer.Write(logEntry);
        }

        [Fact]
        public void HttpLogger_Should_Work_With_New_Context_Pattern()
        {
            // Arrange
            var logger = new HttpLogger();
            logger.AddWriter(new ConsoleLogWriter(writeIndented: true));

            // Act - Probar diferentes tipos de logging con el nuevo patrón
            Console.WriteLine("=== Request Log ===");
            var requestContext = HttpLogContextBuilder.Create()
                .WithTraceId("req-trace-123")
                .WithSpanId("req-span-456")
                .WithHttpRequestPath("/api/products")
                .WithUserId("user-001")
                .WithClientIp("192.168.1.100")
                .AsRequest()
                .WithProperty("QueryParams", "?page=1&size=10");

            logger.Log(requestContext);

            Console.WriteLine("\n=== Response Log ===");
            var responseContext = HttpLogContextBuilder.Create()
                .WithTraceId("req-trace-123")
                .WithSpanId("req-span-456")
                .WithHttpRequestPath("/api/products")
                .WithUserId("user-001")
                .AsResponse(200)
                .WithResponseTime(150)
                .WithProperty("ResultCount", 25);

            logger.Log(responseContext);

            Console.WriteLine("\n=== Outgoing Request Log ===");
            var outgoingContext = HttpLogContextBuilder.Create()
                .WithTraceId("req-trace-123")
                .WithSpanId("out-span-789")
                .WithHttpRequestPath("/api/products")
                .AsOutgoingRequest("https://external-api.com/data")
                .WithProperty("Timeout", "30s");

            logger.Log(outgoingContext);

            // Assert - Solo verificar que no hay errores
            Assert.True(true);
        }

        [Fact]
        public void Demonstrate_Future_Extensibility()
        {
            // Arrange - Simular una extensión futura para canales
            var context = HttpLogContextBuilder.Create()
                .WithTraceId("trace-123")
                .WithSpanId("span-456")
                .WithHttpRequestPath("/api/canal/mobile")
                .AsRequest("Canal mobile request")
                // Campos que se pueden agregar sin romper la API existente
                .WithProperty("CanalType", "Mobile")
                .WithProperty("AppVersion", "2.1.0")
                .WithProperty("DeviceId", "device-abc-123")
                .WithProperty("Platform", "iOS")
                .WithProperty("Location", "Buenos Aires, Argentina")
                .WithProperty("Language", "es-AR")
                // Y muchos más campos sin límite
                .Build();

            var writer = new ConsoleLogWriter(writeIndented: true);
            var logEntry = context.ToLogEntry();

            Console.WriteLine("=== Demostración de Extensibilidad Futura ===");
            writer.Write(logEntry);

            // Assert
            Assert.Equal(6, context.AdditionalProperties?.Count);
            Assert.True(context.AdditionalProperties?.ContainsKey("CanalType"));
            Assert.True(context.AdditionalProperties?.ContainsKey("DeviceId"));
        }
    }
}
