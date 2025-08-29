#!/bin/bash

# 🧪 Script de Testing Completo - POC Arquetipo Logging
# Consolida la funcionalidad de test-api.sh y test-filtered-logging.sh

set -e  # Exit on any error

API_PORT=5040
API_URL="http://localhost:$API_PORT"

echo "🚀 === POC Arquetipo Logging - Test Suite ==="
echo ""

# Función para verificar si la API está corriendo
check_api_running() {
    if curl -s "$API_URL/weatherforecast" > /dev/null 2>&1; then
        return 0
    else
        return 1
    fi
}

# Función para esperar que la API esté lista
wait_for_api() {
    echo "⏳ Esperando que la API esté lista en $API_URL..."
    local max_attempts=30
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        if check_api_running; then
            echo "✅ API está lista!"
            return 0
        fi
        echo "   Intento $attempt/$max_attempts..."
        sleep 1
        ((attempt++))
    done
    
    echo "❌ API no está disponible después de $max_attempts segundos"
    return 1
}

# Función para probar endpoint y mostrar resultado
test_endpoint() {
    local endpoint=$1
    local description=$2
    
    echo "🔍 Probando: $description"
    echo "   Endpoint: $API_URL$endpoint"
    
    local response=$(curl -s -w "\n--- STATUS: %{http_code} | TIME: %{time_total}s ---" "$API_URL$endpoint" 2>/dev/null)
    
    if [ $? -eq 0 ]; then
        echo "✅ Respuesta recibida:"
        echo "$response" | head -10
        if [ $(echo "$response" | wc -l) -gt 10 ]; then
            echo "   ... (respuesta truncada)"
        fi
    else
        echo "❌ Error al conectar con $endpoint"
    fi
    echo ""
}

# Función para iniciar API si no está corriendo
start_api_if_needed() {
    if check_api_running; then
        echo "✅ API ya está corriendo en $API_URL"
        return 0
    fi
    
    echo "🚀 Iniciando API en puerto $API_PORT..."
    dotnet run --project samples/SampleApi/SampleApi.csproj --urls "$API_URL" > /dev/null 2>&1 &
    API_PID=$!
    
    if wait_for_api; then
        echo "✅ API iniciada correctamente (PID: $API_PID)"
        return 0
    else
        echo "❌ Error al iniciar la API"
        return 1
    fi
}

# Main test execution
main() {
    echo "📋 Verificando estado de la API..."
    
    # Intentar conectar a la API existente o iniciarla
    if ! start_api_if_needed; then
        echo "❌ No se pudo iniciar la API. Verifica que .NET esté instalado y el proyecto compile."
        exit 1
    fi
    
    echo ""
    echo "🧪 === INICIANDO PRUEBAS DE LOGGING ==="
    echo ""
    
    # Test 1: Endpoint básico
    test_endpoint "/weatherforecast" "Weather Forecast (logging básico con filtros)"
    
    # Test 2: Endpoint con HTTP interceptors
    test_endpoint "/external-data" "External Data (HTTP interceptors + outgoing requests)"
    
    # Test 3: Endpoint inexistente (error handling)
    test_endpoint "/nonexistent" "Endpoint inexistente (manejo de errores)"
    
    # Test 4: Optimización endpoints (si existen)
    test_endpoint "/api/optimization/with-nulls" "Optimización con nulls (si existe)"
    test_endpoint "/api/optimization/minimal" "Optimización minimal (si existe)"
    
    echo "📊 === RESULTADOS ESPERADOS ==="
    echo ""
    echo "✅ Características del logging verificadas:"
    echo "   • Logs de Microsoft/System suprimidos (SuppressSystemLogs=true)"
    echo "   • Solo logs del arquetipo visible (prefijo 'Bgba')"
    echo "   • Propiedad 'loggerName' incluida automáticamente"
    echo "   • JSON formateado con indentación para legibilidad"
    echo "   • HTTP interceptors capturan requests/responses"
    echo "   • Correlación con TraceId/SpanId funcionando"
    echo ""
    
    if [ ! -z "$API_PID" ]; then
        echo "🛑 Deteniendo API iniciada por el script..."
        kill $API_PID 2>/dev/null || true
        echo "✅ API detenida"
    else
        echo "ℹ️  API sigue corriendo (no fue iniciada por este script)"
    fi
    
    echo ""
    echo "🎉 Suite de pruebas completada!"
    echo "💡 Para más detalles, revisa los logs en la consola de la API"
}

# Ejecutar main si se llama directamente
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
