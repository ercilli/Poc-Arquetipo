# 🐛 Debugging Guide - POC Arquetipo Logging

## 🚀 Configuración Completa para Visual Studio Code

### 📋 Prerequisitos
- Visual Studio Code instalado
- Extensión C# Dev Kit (ms-dotnettools.csharp)
- .NET 8.0 SDK

### 🔧 Configuración de Debugging

#### 1. **Launch Configurations** (`.vscode/launch.json`)
```json
{
    "name": "Debug Sample API",
    "type": "coreclr",
    "request": "launch",
    "preLaunchTask": "build",
    "program": "${workspaceFolder}/samples/SampleApi/bin/Debug/net8.0/SampleApi.dll"
}
```

#### 2. **Build Tasks** (`.vscode/tasks.json`)
- `build`: Construye la solución completa
- `watch`: Modo de desarrollo con hot reload
- `clean`: Limpia los artefactos de build
- `restore`: Restaura las dependencias NuGet

### 🎯 Cómo Debuggear

#### **Opción 1: Debugging con F5**
1. Abre Visual Studio Code en la carpeta del proyecto
2. Ve a la pestaña "Run and Debug" (Ctrl+Shift+D)
3. Selecciona "Debug Sample API" en el dropdown
4. Presiona F5 o click en "Start Debugging"

#### **Opción 2: Debugging Manual**
1. Pon breakpoints en el código (click en el margen izquierdo)
2. Ejecuta el comando: `Debug: Start Debugging` (F5)
3. La aplicación se iniciará en modo debug

#### **Opción 3: Attach to Process**
1. Ejecuta la aplicación manualmente: `dotnet run --project samples/SampleApi/SampleApi.csproj`
2. En VS Code: Run and Debug → "Attach to Process"
3. Selecciona el proceso `SampleApi`

### 🔍 Breakpoints Sugeridos

#### **En ApiController.cs:**
- **Línea ~27**: Antes del log de éxito
- **Línea ~35**: Antes de retornar el resultado exitoso
- **Línea ~47**: Antes de lanzar la excepción
- **Línea ~53**: En el catch de la excepción
- **Línea ~80**: En llamadas HTTP externas

### 📊 Testing con Debugging

#### **1. Test Endpoint Exitoso**
```bash
curl http://localhost:5040/api/success
```
**Debugging**: Pon breakpoint en `Success()` para ver el flujo completo

#### **2. Test Endpoint con Error**
```bash
curl http://localhost:5040/api/error
```
**Debugging**: Pon breakpoint en `ThrowError()` para inspeccionar la excepción

#### **3. Test Llamada Externa**
```bash
curl http://localhost:5040/api/external-call
```
**Debugging**: Pon breakpoint en `CallExternalApi()` para ver la integración HTTP

#### **4. Test Logs Personalizados**
```bash
curl -X POST http://localhost:5040/api/log-error \
  -H "Content-Type: application/json" \
  -d '{"errorType":"critical","message":"Test debug","userId":"debug123"}'
```
**Debugging**: Pon breakpoint en `LogError()` para ver el procesamiento de logs

### 🔧 Variables a Inspeccionar

#### **En tiempo de debugging, inspecciona:**
- `_logger`: Estado del logger y configuración
- `request`: Datos de entrada HTTP
- `ex`: Detalles de excepciones
- `response`: Respuesta de APIs externas
- `loggingConfig`: Configuración de optimización

### 🚨 Debugging de Logs Optimizados

#### **Para verificar optimización de null properties:**
1. Pon breakpoint antes de `_logger.LogInformation()`
2. Inspecciona el objeto que se va a loggear
3. Verifica que propiedades null no aparezcan en el JSON final

#### **Variables a monitorear:**
```csharp
// En el debugger, evalúa estas expresiones:
loggingConfig.ExcludeNullProperties  // true
loggingConfig.ExcludeEmptyStrings    // false  
loggingConfig.MinimalOutput          // false (dev) / true (prod)
```

### ⚡ Hot Reload durante Debugging

#### **Usar watch task para desarrollo rápido:**
```bash
# Terminal en VS Code:
dotnet watch run --project samples/SampleApi/SampleApi.csproj
```

### 🔍 Debugging de Middleware

#### **Para debuggear el middleware de logging:**
1. Pon breakpoints en:
   - `HttpLoggingMiddleware.InvokeAsync()`
   - `CanalLoggingMiddleware.InvokeAsync()`
2. Verifica el enriquecimiento automático de contexto

### 📝 Tips de Debugging

- **Usa el integrated terminal** para ver logs en tiempo real
- **Inspecciona el call stack** para entender el flujo de middleware
- **Evalúa expresiones** en el debug console para probar configuraciones
- **Usa conditional breakpoints** para casos específicos

---

## 🎯 Próximos Pasos
1. Ejecuta el debugger con F5
2. Prueba cada endpoint con breakpoints
3. Inspecciona los logs optimizados
4. Verifica el funcionamiento del middleware de logging
