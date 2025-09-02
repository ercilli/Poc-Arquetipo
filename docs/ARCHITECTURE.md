# 🏗️ Arquitectura del Sistema - POC Arquetipo Logging

## 📋 Índice
1. [Vista General](#-vista-general)
2. [Diagrama de Clases](#-diagrama-de-clases)
3. [Diagrama de Componentes](#-diagrama-de-componentes)
4. [Flujo de Extensión](#-flujo-de-extensión)
5. [Restricciones y Límites](#-restricciones-y-límites)
6. [Guía de Extensión](#-guía-de-extensión)

## 🔍 Vista General

### Arquitectura en Capas
```mermaid
graph TB
    subgraph "🎯 APPLICATION LAYER"
        API[Sample API]
        CTL[Controllers]
    end
    
    subgraph "🔌 EXTENSION LAYER"
        HTTP[bgba-arquetipo-http]
        CANAL[bgba-arquetipo-canales]
        FUTURE[Futuras Extensiones...]
    end
    
    subgraph "⚡ CORE LAYER"
        CORE[Logging.Core]
        CONFIG[LoggingConfiguration]
        OPT[OptimizedJsonSerializer]
    end
    
    subgraph "🏦 INFRASTRUCTURE"
        CONSOLE[Console Output]
        FILES[File Output]
        REMOTE[Remote Logging]
    end
    
    API --> HTTP
    API --> CANAL
    HTTP --> CORE
    CANAL --> CORE
    FUTURE --> CORE
    CORE --> CONSOLE
    CORE --> FILES
    CORE --> REMOTE
    
    classDef appLayer fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    classDef extLayer fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px
    classDef coreLayer fill:#fff3e0,stroke:#f57c00,stroke-width:3px
    classDef infraLayer fill:#e8f5e8,stroke:#388e3c,stroke-width:2px
    
    class API,CTL appLayer
    class HTTP,CANAL,FUTURE extLayer
    class CORE,CONFIG,OPT coreLayer
    class CONSOLE,FILES,REMOTE infraLayer
```

## 🎨 Diagrama de Clases

### Core Components
```mermaid
classDiagram
    %% CORE INTERFACES
    class ILogger {
        <<interface>>
        +Log(LogEntry entry)
        +Log(LogLevel level, string message)
        +Trace(string message)
        +Debug(string message)
        +Information(string message)
        +Warning(string message)
        +Error(string message)
        +Critical(string message)
    }
    
    class ILogWriter {
        <<interface>>
        +Write(LogEntry entry)
    }
    
    class ILogFilter {
        <<interface>>
        +ShouldLog(LogEntry entry, string loggerName)
    }
    
    %% CORE IMPLEMENTATIONS
    class Logger {
        -List~ILogWriter~ _writers
        +AddWriter(ILogWriter writer)
        +Log(LogEntry entry)
    }
    
    class LogEntry {
        +DateTime Timestamp
        +LogLevel Level
        +string Message
        +Dictionary~string,object~ Properties
    }
    
    class ConsoleLogWriter {
        -LoggingConfiguration _config
        -ILogFilter _logFilter
        -JsonSerializerOptions _jsonOptions
        +Write(LogEntry entry)
    }
    
    class LoggingConfiguration {
        +bool WriteIndented
        +bool IncludeTimestamp
        +LogLevel MinimumLevel
        +bool SuppressSystemLogs
        +bool ExcludeNullProperties
        +bool ExcludeEmptyStrings
        +bool MinimalOutput
        +List~string~ ExcludedPropertyNames
    }
    
    %% RELATIONSHIPS
    Logger ..|> ILogger
    Logger --> ILogWriter
    ConsoleLogWriter ..|> ILogWriter
    ConsoleLogWriter --> LoggingConfiguration
    ConsoleLogWriter --> ILogFilter
    Logger --> LogEntry
```

### HTTP Extension
```mermaid
classDiagram
    %% HTTP EXTENSION
    class IHttpLogger {
        <<interface>>
        +Log(HttpLogContext context)
        +LogRequest(HttpLogContext context)
        +LogResponse(HttpLogContext context)
        +LogOutgoingRequest(HttpLogContext context)
        +LogOutgoingResponse(HttpLogContext context)
    }
    
    class HttpLogger {
        +Log(HttpLogContext context)
        +LogRequest(HttpLogContext context)
        +LogResponse(HttpLogContext context)
    }
    
    class HttpLogEntry {
        +string RequestPath
        +string HttpMethod
        +int StatusCode
        +long ResponseTime
        +string TraceId
        +string SpanId
        +LogType LogType
        +string OutgoingRequestPath
    }
    
    class HttpLogContext {
        +LogType LogType
        +string TraceId
        +string SpanId
        +string HttpRequestPath
        +string OutgoingRequestPath
        +int StatusCode
        +long ResponseTime
        +DateTime Timestamp
        +LogLevel Level
        +string Message
    }
    
    class HttpLogContextBuilder {
        +WithLogType(LogType type)
        +WithTraceId(string id)
        +WithSpanId(string id)
        +WithHttpRequestPath(string path)
        +WithStatusCode(int code)
        +Build()
    }
    
    class HttpLoggingMiddleware {
        -RequestDelegate _next
        -IHttpLogger _logger
        +InvokeAsync(HttpContext context)
    }
    
    class HttpLoggingHandler {
        -IHttpLogger _logger
        +SendAsync(HttpRequestMessage request)
    }
    
    %% RELATIONSHIPS
    HttpLogger ..|> IHttpLogger
    HttpLogger --|> Logger
    HttpLogEntry --|> LogEntry
    HttpLogContext --> HttpLogEntry
    HttpLogContextBuilder --> HttpLogContext
    HttpLoggingMiddleware --> IHttpLogger
    HttpLoggingHandler --> IHttpLogger
```

### Canal Enrichment Pattern
```mermaid
classDiagram
    %% ENRICHMENT PATTERN
    class IHttpLogEnricher {
        <<interface>>
        +int Priority
        +EnrichHttpLog(HttpLogEntry entry, HttpContext context)
    }
    
    class CanalHttpLogEnricher {
        +int Priority
        +EnrichHttpLog(HttpLogEntry entry, HttpContext context)
        -ExtractCanalId(HttpContext context)
        -ExtractCanalType(HttpContext context)
        -DetermineOperationType(string path)
    }
    
    class HttpLogEntry {
        %% HTTP Base Properties
        +string Method
        +string Path
        +int StatusCode
        +long Duration
        
        %% Enrichable Canal Properties
        +string? CanalId
        +string? CanalType
        +string? SessionId
        +string? OperationType
        +string? RemoteIp
        +string? UserAgent
    }
    
    class HttpLoggerWithEnrichment {
        -IEnumerable~IHttpLogEnricher~ _enrichers
        +Log(HttpLogEntry entry, HttpContext context)
        -ApplyEnrichments(HttpLogEntry entry, HttpContext context)
    }
    
    class ServiceCollectionExtensions {
        <<static>>
        +AddCanalEnrichment(IServiceCollection services)
    }
    
    %% RELATIONSHIPS
    CanalHttpLogEnricher ..|> IHttpLogEnricher
    HttpLoggerWithEnrichment --> IHttpLogEnricher
    HttpLoggerWithEnrichment --> HttpLogEntry
    CanalHttpLogEnricher --> HttpLogEntry
    ServiceCollectionExtensions --> CanalHttpLogEnricher
```

## 🧩 Diagrama de Componentes

```mermaid
graph LR
    subgraph "📱 SAMPLE API"
        WEB[Web Application]
        API_CTL[API Controllers]
        MID[Middleware Pipeline]
    end
    
    subgraph "🔌 HTTP EXTENSION (with Enrichment)"
        HTTP_LOG[HttpLoggerWithEnrichment]
        HTTP_MID[HttpLoggingMiddleware]
        HTTP_HAND[HttpLoggingHandler]
        HTTP_CTX[HttpLogContext]
        HTTP_ENRICHER[IHttpLogEnricher]
    end
    
    subgraph "🏛️ CANAL ENRICHMENT"
        CANAL_ENRICHER[CanalHttpLogEnricher]
        CANAL_EXTENSIONS[ServiceCollectionExtensions]
    end
    
    subgraph "⚡ LOGGING CORE"
        CORE_LOG[Logger]
        WRITER[ConsoleLogWriter]
        CONFIG[LoggingConfiguration]
        FILTER[DefaultLogFilter]
        OPT[OptimizedJsonSerializer]
    end
    
    subgraph "🏦 OUTPUT TARGETS"
        CONSOLE["🖥️ Console"]
        FILES["📁 Files"]
        REMOTE["🌐 Remote Systems"]
    end
    
    %% CONNECTIONS
    WEB --> API_CTL
    API_CTL --> HTTP_LOG
    MID --> HTTP_MID
    MID --> CANAL_MID
    
    HTTP_LOG --> CORE_LOG
    HTTP_MID --> HTTP_CTX
    HTTP_HAND --> HTTP_CTX
    HTTP_CTX --> HTTP_LOG
    
    CANAL_LOG --> CORE_LOG
    CANAL_MID --> CANAL_CTX
    CANAL_CTX --> CANAL_LOG
    
    CORE_LOG --> WRITER
    WRITER --> CONFIG
    WRITER --> FILTER
    WRITER --> OPT
    
    WRITER --> CONSOLE
    WRITER --> FILES
    WRITER --> REMOTE
    
    classDef apiComp fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    classDef httpComp fill:#fff3e0,stroke:#f57c00,stroke-width:2px
    classDef canalComp fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px
    classDef coreComp fill:#e8f5e8,stroke:#388e3c,stroke-width:3px
    classDef outputComp fill:#fce4ec,stroke:#c2185b,stroke-width:2px
    
    class WEB,API_CTL,MID apiComp
    class HTTP_LOG,HTTP_MID,HTTP_HAND,HTTP_CTX httpComp
    class CANAL_LOG,CANAL_MID,CANAL_CTX canalComp
    class CORE_LOG,WRITER,CONFIG,FILTER,OPT coreComp
    class CONSOLE,FILES,REMOTE outputComp
```

## 🏗️ Cross-Cutting Concerns

### **Capas Transversales**
```mermaid
graph TB
    subgraph "Cross-Cutting Concerns Layer"
        LoggingCore["🔧 Logging.Core<br/>Base Architecture"]
        Security["🔒 Security Logging"]
        Performance["⚡ Performance Monitoring"]
        Audit["📋 Audit Trail"]
        ErrorHandling["❌ Error Handling"]
        Correlation["🔗 Correlation & Tracing"]
    end

    subgraph "Extension Libraries"
        HttpLogging["🌐 bgba-arquetipo-http<br/>HTTP Interceptors"]
        CanalLogging["📱 bgba-arquetipo-canales<br/>Channel-specific"]
        DatabaseLogging["🗄️ bgba-arquetipo-database<br/>Database Operations"]
        ExternalLogging["🌍 bgba-arquetipo-external<br/>External APIs"]
    end

    subgraph "Application Layers"
        PresentationLayer["🖥️ Presentation Layer<br/>Controllers, APIs"]
        BusinessLayer["💼 Business Logic Layer<br/>Services, Domain"]
        DataLayer["📊 Data Access Layer<br/>Repositories, DAOs"]
        InfrastructureLayer["🏗️ Infrastructure Layer<br/>External Services"]
    end

    %% Core integrations
    Security --> LoggingCore
    Performance --> LoggingCore
    Audit --> LoggingCore
    ErrorHandling --> LoggingCore
    Correlation --> LoggingCore

    %% Extension integrations
    HttpLogging --> PresentationLayer
    CanalLogging --> PresentationLayer
    DatabaseLogging --> DataLayer
    ExternalLogging --> InfrastructureLayer

    classDef coreClass fill:#e1f5fe,stroke:#01579b,stroke-width:3px
    classDef extensionClass fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef layerClass fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px

    class LoggingCore,Security,Performance,Audit,ErrorHandling,Correlation coreClass
    class HttpLogging,CanalLogging,DatabaseLogging,ExternalLogging extensionClass
    class PresentationLayer,BusinessLayer,DataLayer,InfrastructureLayer layerClass
```

## 🔄 Flujo de Extensión

### Patrón de Extensión para Nuevos Dominios
```mermaid
sequenceDiagram
    participant DEV as Desarrollador
    participant EXT as Nueva Extensión
    participant CTX as Contexto Específico
    participant LOG as Logger Específico
    participant CORE as Logging.Core
    participant OUT as Output
    
    Note over DEV,OUT: Creación de Nueva Extensión
    
    DEV->>EXT: 1. Crear proyecto bgba-arquetipo-[dominio]
    DEV->>CTX: 2. Extender LogEntry → [Dominio]LogEntry
    DEV->>CTX: 3. Crear [Dominio]LogContext
    DEV->>CTX: 4. Crear [Dominio]LogContextBuilder
    DEV->>LOG: 5. Extender Logger → [Dominio]Logger
    DEV->>EXT: 6. Crear [Dominio]Middleware (opcional)
    DEV->>EXT: 7. Crear ServiceCollectionExtensions
    
    Note over DEV,OUT: Uso en Runtime
    
    EXT->>CTX: 8. Construir contexto específico
    CTX->>LOG: 9. Enviar a logger específico
    LOG->>CORE: 10. Enviar LogEntry enriquecido
    CORE->>OUT: 11. Aplicar optimizaciones y escribir
```

## 🔧 Funcionalidades de Control

### **Sistema de Filtros Avanzado**
```csharp
var loggingConfig = new LoggingConfiguration
{
    WriteIndented = true,
    SuppressSystemLogs = true,              // Suprimir logs de Microsoft/System
    OnlyArquetipoLogs = false,              // Si es true, SOLO muestra logs del arquetipo
    ArquetipoPrefix = "Bgba",               // Prefijo que identifica componentes del arquetipo
    ExcludedLoggerPrefixes = new List<string>
    {
        "Microsoft",                        // Excluir Microsoft.*
        "System",                          // Excluir System.*
        "Microsoft.Extensions",            // Excluir Microsoft.Extensions.*
        "Microsoft.AspNetCore",           // Excluir Microsoft.AspNetCore.*
        "Microsoft.EntityFrameworkCore",  // Excluir EF Core logs
        "Microsoft.Hosting"               // Excluir hosting logs
    }
};
```

### **Detección Automática del Logger Name**
Cada log incluye automáticamente el nombre de la clase que lo generó:

```json
{
  "logType": "REQUEST",
  "traceId": "abc-123",
  "spanId": "def-456", 
  "httpRequestPath": "/api/users",
  "loggerName": "UserController",          // ← Nombre de la clase detectado automáticamente
  "timestamp": "2025-08-28T15:30:00.000Z",
  "level": "Information",
  "message": "Processing user request"
}
```

### **Configuraciones por Ambiente**
```csharp
// Producción (Máximo control)
var prodConfig = new LoggingConfiguration
{
    WriteIndented = false,
    SuppressSystemLogs = true,
    OnlyArquetipoLogs = true,             // SOLO arquetipo
    ArquetipoPrefix = "Bgba",
    MinimumLevel = LogLevel.Information
};

// Desarrollo (Debugging habilitado)  
var devConfig = new LoggingConfiguration
{
    WriteIndented = true,                 // JSON legible
    SuppressSystemLogs = true,            // Sin ruido del sistema
    OnlyArquetipoLogs = false,            // Incluir algunos logs útiles
    ArquetipoPrefix = "Bgba",
    MinimumLevel = LogLevel.Trace
};
```

## 📊 Casos de Uso Empresariales

### **Matriz de Implementación**
```mermaid
graph TB
    subgraph "Escenarios Empresariales"
        Banking["🏦 Sistema Bancario<br/>• Alta Seguridad<br/>• Cumplimiento Regulatorio<br/>• Detección de Fraude"]
        Healthcare["🏥 Plataforma Salud<br/>• HIPAA Compliance<br/>• Privacidad Pacientes<br/>• Auditoría"]
        Ecommerce["🛒 E-commerce<br/>• Alto Volumen<br/>• Monitoreo Performance<br/>• Analytics Cliente"]
        Government["🏛️ Sistema Gobierno<br/>• Clasificación Datos<br/>• Retención Long-term<br/>• Seguridad Nacional"]
    end

    subgraph "Extensiones Requeridas"
        SecurityExt["🔒 Extensión Seguridad<br/>• Logs Autenticación<br/>• Control Acceso<br/>• Detección Amenazas"]
        ComplianceExt["📜 Extensión Compliance<br/>• Reportes Regulatorios<br/>• Retención Datos<br/>• Audit Trails"]
        PerformanceExt["⚡ Extensión Performance<br/>• Métricas Colección<br/>• Monitoreo SLA<br/>• Detección Cuellos"]
        BusinessExt["💼 Extensión Negocio<br/>• Eventos Negocio<br/>• Tracking KPI<br/>• Impacto Revenue"]
    end

    Banking --> SecurityExt
    Banking --> ComplianceExt
    Healthcare --> ComplianceExt
    Healthcare --> SecurityExt
    Ecommerce --> PerformanceExt
    Ecommerce --> BusinessExt
    Government --> SecurityExt
    Government --> ComplianceExt

    classDef scenarioClass fill:#e8f5e8,stroke:#2e7d32,stroke-width:2px
    classDef extensionClass fill:#fff3e0,stroke:#e65100,stroke-width:2px

    class Banking,Healthcare,Ecommerce,Government scenarioClass
    class SecurityExt,ComplianceExt,PerformanceExt,BusinessExt extensionClass
```

### **Stack Tecnológico Integrado**
```mermaid
graph LR
    subgraph "Application Layer"
        AspNetCore["ASP.NET Core"]
        WebAPI["Web API"]
        BlazorServer["Blazor Server"]
    end

    subgraph "Logging.Core Ecosystem"
        LoggingCore["📝 Logging.Core"]
        HttpExt["🌐 HTTP Extension"]
        CanalExt["📱 Canal Extension"]
        DbExt["🗄️ Database Extension"]
    end

    subgraph "Infrastructure"
        Docker["🐳 Docker"]
        Kubernetes["☸️ Kubernetes"]
        Azure["☁️ Azure"]
        ElasticStack["🔍 ELK Stack"]
    end

    AspNetCore --> LoggingCore
    WebAPI --> LoggingCore
    BlazorServer --> LoggingCore

    LoggingCore --> HttpExt
    LoggingCore --> CanalExt
    LoggingCore --> DbExt

    LoggingCore --> Docker
    LoggingCore --> Kubernetes
    LoggingCore --> Azure
    LoggingCore --> ElasticStack

    classDef appClass fill:#e8f5e8,stroke:#2e7d32,stroke-width:2px
    classDef coreClass fill:#e1f5fe,stroke:#01579b,stroke-width:3px
    classDef infraClass fill:#f3e5f5,stroke:#4a148c,stroke-width:2px

    class AspNetCore,WebAPI,BlazorServer appClass
    class LoggingCore,HttpExt,CanalExt,DbExt coreClass
    class Docker,Kubernetes,Azure,ElasticStack infraClass
```

### **Checklist de Implementación**
#### ✅ **Infraestructura Core** (Completado)
- [x] Arquitectura básica Logging.Core
- [x] Extensión HTTP con middleware
- [x] Patrones Context y Builder
- [x] Gestión de configuración
- [x] Framework de testing

#### 🔄 **Desarrollo Actual** (En Progreso)
- [x] HTTP interceptors mejorados
- [x] Optimización performance
- [x] Integración seguridad
- [x] Enriquecimiento contexto avanzado

#### 📋 **Extensiones Planificadas**
- [ ] Extensión logging database
- [ ] Extensión message queue
- [ ] Logging operaciones cache
- [ ] Extensiones específicas por canal
- [ ] Logging APIs externas

## 🎯 Restricciones Arquitectónicas

### 🚫 **LO QUE NO SE PUEDE HACER**

#### **1. Violaciones de Arquitectura**
```csharp
// ❌ PROHIBIDO: Extensión accediendo directamente a Core internals
public class BadExtension 
{
    public void BadMethod() 
    {
        // No acceder directamente a ConsoleLogWriter
        var writer = new ConsoleLogWriter(); // ❌ PROHIBIDO
        
        // No modificar LoggingConfiguration desde extensión
        config.InternalMethod(); // ❌ PROHIBIDO
    }
}
```

#### **2. Dependencias Circulares**
```csharp
// ❌ PROHIBIDO: Logging.Core NO puede depender de extensiones
namespace Logging.Core 
{
    public class Logger 
    {
        // ❌ No referenciar bgba-arquetipo-http
        private HttpLogger _httpLogger; // ❌ PROHIBIDO
    }
}
```

#### **3. Modificación de Clases Core**
```csharp
// ❌ PROHIBIDO: Modificar LogEntry directamente
public class LogEntry 
{
    // ❌ No agregar propiedades específicas de dominio aquí
    public string HttpMethod { get; set; } // ❌ PROHIBIDO
    public string CanalId { get; set; } // ❌ PROHIBIDO
}
```

### ✅ **LO QUE SÍ SE PUEDE HACER**

#### **1. Extensión Correcta**
```csharp
// ✅ CORRECTO: Extensión por herencia
public class DatabaseLogEntry : LogEntry 
{
    public string ConnectionString { get; set; }
    public string Query { get; set; }
    public long ExecutionTime { get; set; }
}
```

#### **2. Dependency Injection Apropiada**
```csharp
// ✅ CORRECTO: Registrar en DI container
public static class ServiceCollectionExtensions 
{
    public static IServiceCollection AddDatabaseLogging(
        this IServiceCollection services, 
        LoggingConfiguration config) 
    {
        services.AddSingleton<IDatabaseLogger>(provider => 
        {
            var logger = new DatabaseLogger();
            logger.AddWriter(new ConsoleLogWriter(config.WriteIndented));
            return logger;
        });
        
        return services;
    }
}
```

### 🎯 **Límites Técnicos**

#### **1. Performance**
- **Máximo 10,000 logs/segundo** por instancia
- **Serialización JSON limitada a 1MB** per log entry
- **Buffer máximo de 100MB** en memoria

#### **2. Configuración**
- **Máximo 50 propiedades** en ExcludedPropertyNames
- **Máximo 10 LogWriters** por Logger instance
- **MinimumStringLength no puede exceder 1000** caracteres

#### **3. Extensión**
- **Máximo 5 niveles de herencia** desde LogEntry
- **Propiedades específicas limitadas a 20** por extensión
- **Nombre de dominio máximo 20 caracteres** (bgba-arquetipo-[max20chars])

## 🛠️ Guía de Extensión

### **Paso a Paso para Nueva Extensión**

#### **1. Setup del Proyecto**
```bash
# Crear nuevo proyecto
dotnet new classlib -n bgba-arquetipo-[dominio]
cd bgba-arquetipo-[dominio]

# Agregar referencia a Logging.Core
dotnet add reference ../Logging.Core/Logging.Core.csproj

# Agregar a solución
dotnet sln ../PoC-Arquetipo-Logging.sln add .
```

#### **2. Estructura Mínima Requerida**
```
bgba-arquetipo-[dominio]/
├── [Dominio]LogEntry.cs        # Extender LogEntry
├── [Dominio]LogContext.cs      # Contexto específico
├── [Dominio]LogContextBuilder.cs # Builder pattern
├── [Dominio]Logger.cs          # Logger específico
├── Extensions/
│   └── ServiceCollectionExtensions.cs # DI setup
└── Middleware/ (opcional)
    └── [Dominio]LoggingMiddleware.cs
```

#### **3. Template de Implementación**
```csharp
// [Dominio]LogEntry.cs
public class [Dominio]LogEntry : LogEntry 
{
    public string [Propiedad1] { get; set; }
    public string [Propiedad2] { get; set; }
    // ... propiedades específicas del dominio
}

// [Dominio]Logger.cs
public class [Dominio]Logger : Logger 
{
    public void Log([Dominio]LogContext context) 
    {
        var entry = context.ToLogEntry();
        base.Log(entry);
    }
}
```

#### **4. Checklist de Validación**
- [ ] ✅ Extiende LogEntry (no modifica)
- [ ] ✅ Implementa Builder pattern
- [ ] ✅ Registra en DI correctamente  
- [ ] ✅ No depende de otras extensiones
- [ ] ✅ Documenta las nuevas propiedades
- [ ] ✅ Incluye tests unitarios
- [ ] ✅ Sigue naming convention bgba-arquetipo-[dominio]

---

## 📚 Referencias

- [Extension Patterns](extension-patterns.md)
- [Banking Optimizations](log-optimization-banking.md)
- [Configuration Guide](../README.md)

---

*📅 Última actualización: 29 de agosto de 2025*
*👨‍💻 Mantenido por: Equipo de Arquitectura*
