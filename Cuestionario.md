# Análisis de Componente: [Nombre del componente]

## 1. Información General
- **Arquitectura principal:** [HTTP / BFF / Batch / MQ / EDA / Streaming / ETL / WebSocket / GraphQL / File-Sync]
- **Responsables del análisis:** [TO / Devs por stack]
- **Descripción breve:** [¿Qué hace el componente? 2–3 líneas]

---

## 2. Clasificación Core / Arquetipo / Extensión
1. ¿Lo necesitan **todas las apps** sin importar arquitectura? → [Sí/No]
2. ¿Define la **forma** de la aplicación (HTTP, Batch, MQ)? → [Sí/No]
3. ¿Depende de **políticas/negocio** o varía por squad? → [Sí/No]
4. ¿Debe poder cambiarse **sin redeploy** vía BO? → [Sí/No + detalle]
5. ¿Contrato debe ser **estable en 4 tecnologías**? → [Sí/No]

**Decisión propuesta:** [Core / Arquetipo / Extensión]  
**Riesgo si cambia:** [Bajo/Medio/Alto]  
**Mitigación:** [Contratos, adaptadores, toggles, SemVer, etc.]

---

## 3. Requerimientos Funcionales
- [Bullet 1]
- [Bullet 2]
- [Bullet 3]

## 4. Requerimientos No Funcionales (NFR)
- Latencia: [Objetivo]
- Seguridad: [Authn/Authz, PII, auditoría]
- Observabilidad: [Logs, métricas, trazas]
- Resiliencia: [Retries, DLQ, circuit breakers]

---

## 5. Dependencias e Integraciones
- [Lista de sistemas / APIs / colas / archivos relacionados]

---

## 6. Entregables por Stack
| Tecnología | Entregable | Notas |
|------------|------------|-------|
| .NET       | [Nombre paquete/librería] | [Detalle] |
| Java       | [Nombre paquete/librería] | [Detalle] |
| Node.js    | [Nombre paquete/librería] | [Detalle] |
| Python     | [Nombre paquete/librería] | [Detalle] |

---

## 7. Configuración BO (si aplica)
- **Flags disponibles:** [ej. `docs.enabled`, `logs.level`, `security.required`]  
- **Valores por ambiente:**  
  - DEV: [valor]  
  - QA: [valor]  
  - PROD: [valor]

---

## 8. Notas / Dudas / ADRs
- [Puntos abiertos para discutir en reunión]