using BgbaArquetipoHttp.Extensions;
using BgbaArquetipoCanales.Extensions;
using Logging.Core;
using SampleApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Suppress default ASP.NET Core logging to console
builder.Logging.ClearProviders();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure logging with banking optimizations
var loggingConfig = new LoggingConfiguration
{
    WriteIndented = true, // Para ver los logs más legibles en desarrollo
    IncludeTimestamp = true,
    MinimumLevel = Logging.Core.LogLevel.Trace,
    SuppressSystemLogs = true, // Suprimir logs de Microsoft y sistema
    OnlyArquetipoLogs = false, // Si quieres SOLO logs del arquetipo, cambiar a true
    ArquetipoPrefix = "Bgba", // Prefijo que identifica componentes del arquetipo

    // 🚀 OPTIMIZACIONES PARA INFRAESTRUCTURA BANCARIA
    ExcludeNullProperties = true,     // No enviar propiedades null (ahorra ancho de banda)
    ExcludeEmptyStrings = false,      // Opcional: omitir strings vacíos
    MinimumStringLength = 0,          // Opcional: mínimo de caracteres para incluir strings
    ExcludedPropertyNames = new()     // Opcional: propiedades siempre excluidas
    {
        // "SensitiveField", "AlwaysEmpty"
    }
    // MinimalOutput = true           // 🔥 Modo ultra-compacto para producción bancaria
};

// Configuración simple: El desarrollador usa _logger.LogInformation() y obtiene contexto automático
builder.Services.AddHttpLogging(loggingConfig);

// Add canal enrichment to HTTP logs instead of separate canal logs
builder.Services.AddCanalEnrichment();

// 🔧 IMPORTANTE: Configurar HttpClient con nuestro interceptor de logging
builder.Services.AddHttpClient("LoggedHttpClient")
    .AddHttpMessageHandler<BgbaArquetipoHttp.Interceptors.HttpLoggingHandler>();

// También agregar el HttpClient básico para inyección directa
builder.Services.AddHttpClient();

// Registrar el HttpLoggingHandler
builder.Services.AddTransient<BgbaArquetipoHttp.Interceptors.HttpLoggingHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add HTTP logging middleware
app.UseBgbaHttpLogging();

// Add canal logging middleware - COMENTADO para usar solo como enriquecimiento
// app.UseCanalLogging();

// Map controllers
app.MapControllers();

// Agregar logging para mostrar el puerto en el que se levanta la aplicación
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetService<Microsoft.AspNetCore.Hosting.Server.IServer>()?.Features?.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;
    if (addresses != null)
    {
        foreach (var address in addresses)
        {
            Console.WriteLine($"🚀 Aplicación iniciada en: {address}");
        }
    }
});

app.Run();
