# POC Arquetipo de Logging

Esta es una Proof of Concept (POC) que demuestra una arquitectura extensible de logging para aplicaciones .NET, compuesta por dos librerías principales:

- **Logging.Core**: Librería base con funcionalidad de logging fundamental
- **bgba-arquetipo-http**: Extensión que agrega capacidades específicas para logging HTTP

## 🏗️ Arquitectura

### 📋 Architecture Documentation
- [**Cross-Cutting Concerns & Architecture Overview**](docs/logging-architecture-diagram.md) - Comprehensive diagrams showing how Logging.Core integrates across different architectural patterns
- [**Extension Patterns & Scalability**](docs/extension-patterns.md) - Detailed patterns for building scalable extensions and multi-tenant support  
- [**Use Cases & Implementation Examples**](docs/use-cases-implementation.md) - Real-world scenarios and implementation roadmap for enterprise applications
- [**🚀 Transparent Logging for Developers**](docs/transparent-logging.md) - Simple API that automatically enriches logs with context
- [**🔧 Canal Extension Pattern**](docs/canal-extension-pattern.md) - Example of domain-specific extensions without modifying base code
- [**💰 Log Optimization for Banking Infrastructure**](docs/log-optimization-banking.md) - Resource optimization features for high-volume environments
- [**⚙️ Environment-Specific Optimization**](docs/environment-specific-optimization.md) - Configuration templates for different environments

### Logging.Core

La librería base proporciona:

- **LogEntry**: Clase base con campos fundamentales:
  - `timestamp`: Marca de tiempo del log
  - `level`: Nivel de severidad del log
  - `message`: Mensaje descriptivo

- **LogLevel**: Enum con niveles estándar (Trace, Debug, Information, Warning, Error, Critical)
- **ILogger/Logger**: Interfaz y implementación base para logging
- **ILogWriter**: Interfaz para escritores de logs (ej. ConsoleLogWriter)

### bgba-arquetipo-http

Extensión que agrega funcionalidad HTTP específica:

- **HttpLogEntry**: Extiende LogEntry con campos HTTP:
  - `log_type`: Tipo de log HTTP (REQUEST, RESPONSE, OUTGOING_REQUEST, OUTGOING_RESPONSE)
  - `trace_id`: Identificador de traza
  - `span_id`: Identificador de span
  - `http_request_path`: Ruta de la request HTTP
  - `outgoing_request_path`: Ruta de requests salientes (opcional)
  - Campos enriquecibles por extensiones:
    - `canal_id`, `canal_type`, `session_id`: Información de canal
    - `operation_type`, `remote_ip`, `user_agent`: Contexto de operación

- **LogType**: Enum con tipos específicos para HTTP
- **HttpLoggingMiddleware**: Middleware para interceptar requests/responses entrantes
- **HttpLoggingHandler**: Handler para interceptar requests/responses salientes
- **IHttpLogger**: Logger especializado para operaciones HTTP
- **Sistema de Enriquecimiento**: Permite que extensiones agreguen campos adicionales sin modificar la estructura base

## 🚀 Quick Start

1. **Clona y construye el proyecto:**
```bash
git clone <repository>
cd Poc-Arquetipo
dotnet build
```

2. **Ejecuta la Sample API:**
```bash
dotnet run --project samples/SampleApi/SampleApi.csproj
```

3. **Prueba los endpoints:**
```bash
# ✅ Endpoint exitoso
curl http://localhost:5040/api/success

# ❌ Endpoint que genera excepción  
curl http://localhost:5040/api/error

# 🌐 Endpoint que llama a API externa
curl http://localhost:5040/api/external-call

# 📝 Endpoint para logs de error personalizados
curl -X POST http://localhost:5040/api/log-error 
  -H "Content-Type: application/json" 
  -d '{"errorType":"critical","message":"Error crítico","userId":"user123"}'
```

4. **Observa los logs optimizados en la consola** - verás que las propiedades null no aparecen, ahorrando recursos de infraestructura.

## 📋 Estructura del Proyecto

```
PoC-Arquetipo-Logging/
├── src/
│   ├── Logging.Core/                 # Librería base de logging
│   │   ├── LogEntry.cs
│   │   ├── LogLevel.cs
│   │   ├── ILogger.cs
│   │   ├── ILogWriter.cs
│   │   └── ConsoleLogWriter.cs
│   └── bgba-arquetipo-http/          # Extensión HTTP
│       ├── HttpLogEntry.cs
│       ├── LogType.cs
│       ├── HttpLogger.cs
│       ├── HttpLoggingMiddleware.cs
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       └── Interceptors/
│           └── HttpLoggingHandler.cs
├── tests/                            # Proyectos de pruebas
│   ├── Logging.Core.Tests/
│   └── bgba-arquetipo-http.Tests/
├── samples/
│   └── SampleApi/                    # API de ejemplo
└── PoC-Arquetipo-Logging.sln
```

## 🔧 Comandos Útiles

```bash
# Compilar toda la solución
dotnet build

# Ejecutar tests
dotnet test

# Ejecutar la API de ejemplo
dotnet run --project samples/SampleApi/SampleApi.csproj

# Crear paquetes NuGet
dotnet pack src/Logging.Core/Logging.Core.csproj
dotnet pack src/bgba-arquetipo-http/bgba-arquetipo-http.csproj
```

## 🔍 Ejemplo de Logs

### Request HTTP entrante enriquecido:
```json
{
  "logType": "REQUEST",
  "traceId": "abc-123-def-456",
  "spanId": "span-789",
  "httpRequestPath": "/api/users",
  "loggerName": "HttpLoggingMiddleware",
  "timestamp": "2025-08-28T15:30:00.123Z",
  "level": "Information", 
  "message": "Incoming request: GET /api/users",
  "canalId": "mobile-app",
  "canalType": "mobile",
  "sessionId": "abc123",
  "operationType": "API_QUERY",
  "remoteIp": "192.168.1.100",
  "userAgent": "MobileApp/1.0"
}
```

### Request HTTP saliente:
```json
{
  "timestamp": "2025-08-27T14:00:01.000Z",
  "level": 2,
  "message": "Outgoing request: GET https://jsonplaceholder.typicode.com/posts/1",
  "logType": 2,
  "traceId": "abc123",
  "spanId": "def456",
  "httpRequestPath": "/external-data",
  "outgoingRequestPath": "https://jsonplaceholder.typicode.com/posts/1"
}
```

## 🎯 Extensibilidad y Enriquecimiento

La arquitectura está diseñada para ser extensible a través de un sistema de enriquecimiento que permite agregar campos específicos sin modificar la estructura base.

### Sistema de Enriquecimiento
```csharp
// Ejemplo: Extensión de canal que enriquece logs HTTP
public class CanalHttpLogEnricher : IHttpLogEnricher
{
    public void EnrichHttpLog(HttpLogEntry entry, HttpContext context)
    {
        // Los campos se agregan directamente al log, no en un objeto anidado
        if (context.Request.Headers.TryGetValue("X-Canal-Id", out var canalId))
            entry.CanalId = canalId.ToString();
            
        if (context.Request.Headers.TryGetValue("X-Canal-Type", out var canalType))
            entry.CanalType = canalType.ToString();
    }
}

// Configuración
services.AddHttpLogging(config);
services.AddCanalEnrichment(); // Registra el enriquecedor automáticamente
```

### Ejemplo de Extensión para Canales
Para crear una extensión `bgba-arquetipo-canales`:

1. **Implementar `IHttpLogEnricher`** para agregar campos específicos
2. **Registrar el enriquecedor** a través de extension methods
3. **Los campos aparecen directamente en el log** como propiedades de primer nivel
4. **Sin logs duplicados** - un solo log HTTP enriquecido

**Resultado**: Los logs HTTP automáticamente incluyen información de canal sin código adicional en controladores.

## 💰 Optimización para Infraestructura Bancaria

### 🚀 Configuración Optimizada
```csharp
var loggingConfig = new LoggingConfiguration
{
    ExcludeNullProperties = true,     // 🎯 No enviar campos null (ahorra ancho de banda)
    ExcludeEmptyStrings = true,       // 🎯 Omitir strings vacíos  
    MinimalOutput = true,             // 🔥 Nombres de campo compactos
    ExcludedPropertyNames = new()     // 🔒 Campos siempre excluidos
    {
        "Password", "Token", "SSN", "InternalDebugInfo"
    }
};
```

### 📊 Impacto en Logs

**❌ Sin Optimización (380 bytes):**
```json
{
  "timestamp": "2025-08-28T10:30:00.000Z",
  "level": "Information",
  "message": "Usuario autenticado",
  "trace_id": "abc123",
  "status_code": null,           // 🚨 DESPERDICIO
  "response_time": null,         // 🚨 DESPERDICIO
  "error_message": null,         // 🚨 DESPERDICIO
  "id_identidad": null          // 🚨 DESPERDICIO
}
```

**✅ Con Optimización (120 bytes - 68% menos):**
```json
{
  "ts": "2025-08-28T10:30:00.000Z",
  "lvl": "Information",
  "msg": "Usuario autenticado",
  "tid": "abc123"
}
```

### 🏦 Beneficios Bancarios
- **💾 Ahorro de Storage**: 360 GB/año en múltiples ambientes
- **🌐 Ancho de Banda**: Menos tráfico entre datacenters  
- **⚡ Performance**: Serialización JSON más rápida
- **🔒 Seguridad**: Exclusión automática de campos sensibles

Ver [documentación completa de optimización](docs/log-optimization-banking.md).

## 🧪 Testing

Los proyectos incluyen configuración básica para pruebas unitarias con xUnit. Para ejecutar:

```bash
dotnet test
```

## 📦 Paquetes NuGet

Las librerías están configuradas para generar paquetes NuGet independientes:

- **Logging.Core**: Paquete base reutilizable
- **bgba-arquetipo-http**: Extensión HTTP que depende de Logging.Core

## 🛠️ Tecnologías

- **.NET 8.0**
- **ASP.NET Core** (para middleware y API)
- **xUnit** (para testing)
- **Swagger/OpenAPI** (para documentación de API)

## 🤝 Contribución

Este es un proyecto POC diseñado para demostrar patrones de arquitectura extensible. Para extender la funcionalidad, sigue los patrones establecidos en las librerías existentes.
