using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SampleApi.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiController(ILogger<ApiController> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Endpoint que devuelve OK - Demuestra logging exitoso
    /// </summary>
    [HttpGet("success")]
    public IActionResult Success()
    {
        // 🔍 BREAKPOINT: Aquí puedes poner un breakpoint para debuggear
        _logger.LogInformation("Operación exitosa ejecutada correctamente");

        var result = new
        {
            status = "success",
            message = "Operación completada exitosamente",
            timestamp = DateTime.UtcNow
        };

        // 🔍 BREAKPOINT: Otro punto para inspeccionar el resultado
        return Ok(result);
    }

    /// <summary>
    /// Endpoint que genera excepción - Demuestra logging de errores
    /// </summary>
    [HttpGet("error")]
    public IActionResult ThrowError()
    {
        try
        {
            // 🔍 BREAKPOINT: Puedes debuggear antes de la excepción
            _logger.LogInformation("Iniciando operación que fallará");

            // Simular una operación que falla
            throw new InvalidOperationException("Error simulado para demostrar logging de excepciones");
        }
        catch (Exception ex)
        {
            // 🔍 BREAKPOINT: Inspeccionar la excepción capturada
            _logger.LogError(ex, "Error crítico en la operación: {ErrorMessage}", ex.Message);

            var errorResult = new
            {
                error = "Internal server error",
                message = ex.Message,
                timestamp = DateTime.UtcNow
            };

            return StatusCode(500, errorResult);
        }
    }

    /// <summary>
    /// Endpoint que llama a otra API - Demuestra logging de llamadas HTTP
    /// </summary>
    [HttpGet("external-call")]
    public async Task<IActionResult> CallExternalApi()
    {
        try
        {
            // 🔍 BREAKPOINT: Inspeccionar antes de la llamada HTTP
            _logger.LogInformation("Iniciando llamada a API externa");

            // Usar el HttpClient con logging configurado
            var loggedHttpClient = _httpClientFactory.CreateClient("LoggedHttpClient");

            // Simular llamada a API externa (JSONPlaceholder como ejemplo)
            var response = await loggedHttpClient.GetAsync("https://jsonplaceholder.typicode.com/posts/1"); if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Llamada a API externa exitosa. Status: {StatusCode}",
                    response.StatusCode);

                return Ok(new
                {
                    status = "success",
                    externalData = JsonDocument.Parse(content).RootElement,
                    statusCode = (int)response.StatusCode,
                    timestamp = DateTime.UtcNow
                });
            }
            else
            {
                _logger.LogWarning("API externa devolvió error. Status: {StatusCode}",
                    response.StatusCode);

                return StatusCode((int)response.StatusCode, new
                {
                    error = "External API error",
                    statusCode = (int)response.StatusCode,
                    timestamp = DateTime.UtcNow
                });
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de conectividad con API externa: {ErrorMessage}", ex.Message);

            return StatusCode(503, new
            {
                error = "Service unavailable",
                message = "No se pudo conectar con la API externa",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en llamada a API externa: {ErrorMessage}", ex.Message);

            return StatusCode(500, new
            {
                error = "Internal server error",
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Endpoint específico para logs de error - Demuestra diferentes niveles de logging
    /// </summary>
    [HttpPost("log-error")]
    public IActionResult LogError([FromBody] LogErrorRequest request)
    {
        _logger.LogInformation("Recibida solicitud de log de error: {ErrorType}", request.ErrorType);

        switch (request.ErrorType?.ToLower())
        {
            case "critical":
                _logger.LogCritical("Error crítico simulado: {Message} | Usuario: {UserId}",
                    request.Message, request.UserId);
                break;

            case "error":
                _logger.LogError("Error simulado: {Message} | Usuario: {UserId}",
                    request.Message, request.UserId);
                break;

            case "warning":
                _logger.LogWarning("Advertencia simulada: {Message} | Usuario: {UserId}",
                    request.Message, request.UserId);
                break;

            case "info":
                _logger.LogInformation("Información simulada: {Message} | Usuario: {UserId}",
                    request.Message, request.UserId);
                break;

            default:
                _logger.LogError("Tipo de error no reconocido: {ErrorType} | Mensaje: {Message}",
                    request.ErrorType, request.Message);
                break;
        }

        return Ok(new
        {
            status = "logged",
            errorType = request.ErrorType,
            message = "Log registrado exitosamente",
            timestamp = DateTime.UtcNow
        });
    }
}

/// <summary>
/// Request model para el endpoint de log de errores
/// </summary>
public class LogErrorRequest
{
    public string? ErrorType { get; set; }
    public string? Message { get; set; }
    public string? UserId { get; set; }
}
