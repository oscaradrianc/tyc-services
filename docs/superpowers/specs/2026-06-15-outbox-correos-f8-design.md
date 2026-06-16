# F8 — Cola de correos persistente (outbox) con reintentos

- **Fecha:** 2026-06-15
- **Repo:** tyc-services (.NET 10 + ServiceStack + Devart LinqConnect)
- **Rama:** `feature/outbox-correos-f8`
- **Origen:** auditoría 2026-06-10, hallazgo alto **F8** (correo de creación fire-and-forget en `Task.Run`, sin cola ni reintento, sin visibilidad → "creé y no llegó el link"). Relacionado: causa #1 del síntoma "al crear cosas no funciona".

## Objetivo

Reemplazar todos los envíos de correo directos/`Task.Run` por un **outbox persistente** drenado por un worker con reintentos y dead-letter, de modo que ningún correo se pierda en silencio y su estado sea observable.

## Decisiones (brainstorming aprobado)

1. **Alcance:** TODOS los correos salientes (creación de consentimiento, reenvío, reset de clave, notificaciones de encuestas) pasan por el outbox.
2. **Contenido persistido:** el correo **ya renderizado** (destinatario + asunto + HTML final + imágenes inline serializadas). El worker solo reconstruye y envía; es agnóstico al tipo de correo.
3. **Reintentos:** backoff exponencial `1m, 5m, 30m, 2h, 6h` (tope 6h), máx 5 intentos; luego `fallido` (dead-letter) visible para reenvío manual.
4. **Visibilidad:** cada fila referencia su origen (`tipo_origen` + `ref_origen`); F8 expone el estado vía un **endpoint dedicado de solo lectura** `GET /consentimientos/{guid}/estado-correo` (devuelve `estado`, `intentos`, `ultimo_error`, `enviado` de la última fila del outbox para ese `ref_origen`). Se elige endpoint dedicado en vez de campo en el listado para no tocar el contrato/consulta del listado existente; el badge en la UI del admin (`tyc-web`) lo consume aparte.
5. **Enfoque:** outbox en BD + `BackgroundService` que sondea (sin infraestructura nueva; patrón de workers ya endurecido en F7).

## 1. Modelo de datos — tabla `email_outbox`

BD transaccional `consentimiento`, estilo moderno snake_case (mapeo Devart como `password_reset_tokens`).

| Columna | Tipo | Para qué |
|---|---|---|
| `id` | bigint PK (db-generated) | — |
| `destinatario` | text | email destino |
| `asunto` | text | subject |
| `cuerpo_html` | text | HTML renderizado (refs `cid:` al logo) |
| `imagenes_json` | text (jsonb) null | imágenes inline `[{content_id, mime, base64}]` |
| `tipo_origen` | text | `CONSENTIMIENTO_CREADO` / `CONSENTIMIENTO_REENVIO` / `PASSWORD_RESET` / `ENCUESTA_NOTIF` |
| `ref_origen` | text null | correlación (GUID consentimiento, id usuario…) |
| `empresa_id` | int null | contexto multi-tenant / filtros |
| `estado` | text | `pendiente` / `enviando` / `enviado` / `fallido` |
| `intentos` | int default 0 | intentos hechos |
| `max_intentos` | int default 5 | tope antes de dead-letter |
| `proximo_intento` | timestamptz | cuándo toca (inicial = `now()`) |
| `ultimo_error` | text null | mensaje del último fallo |
| `creado` | timestamptz | alta |
| `enviado` | timestamptz null | cuándo se envió |

**Estados:** `pendiente` → `enviando` (claim) → `enviado`; o vuelve a `pendiente` con `proximo_intento` aplazado; o `fallido` al agotar `max_intentos`.

**Índices:**
- `(estado, proximo_intento)` → consulta de drenado.
- `(tipo_origen, ref_origen)` → lookup de estado por origen.

**Despliegue:** script DDL en `scripts/` (lo aplica Adrian; los MCP son read-only). No hay framework de migraciones; Devart solo mapea.

## 2. Componentes de encolado

- **Entidad** `EmailOutbox` (`Tyc.Modelo/Contexto/EmailOutbox.cs`) — mapeo Devart.
- **Interfaz** `IEmailOutbox` (`Tyc.Interface/Services/`):

```csharp
public interface IEmailOutbox
{
    // Inserta una fila 'pendiente' en la MISMA transacción de negocio (atómico).
    Task EncolarAsync(TycBaseContext ctx, EmailOutboxItem item);
}

public record EmailOutboxItem(
    string Destinatario, string Asunto, string CuerpoHtml,
    IReadOnlyList<ImagenEnLinea> Imagenes,   // -> imagenes_json
    string TipoOrigen, string? RefOrigen, int? EmpresaId);
```

- **Implementación** `EmailOutboxService` (`Tyc.Implementacion/Email/`): serializa `Imagenes` a JSON, crea la fila `pendiente` (`intentos=0`, `proximo_intento=now()`), `SubmitChanges` sobre el `ctx` recibido. Registrada en DI como `IEmailOutbox`.
- **El render se queda en cada BL** (conoce su plantilla). Solo cambia "enviar ahora" por "encolar". El worker reconstruye el `AlternateView` con `ConstruirVistaConImagen`.

## 3. Worker de drenado — `EmailOutboxWorker`

`BackgroundService` (`FrameAppWS/Workers/`), patrón F7 (try/catch dentro del bucle, scope manual, conexión fija `Consentimiento`). Ciclo cada ~15s:

1. **Reclamar lote** (atómico, anti doble-envío):
   `UPDATE email_outbox SET estado='enviando' WHERE id IN (SELECT id FROM email_outbox WHERE estado='pendiente' AND proximo_intento <= now() ORDER BY creado LIMIT 20 FOR UPDATE SKIP LOCKED) RETURNING ...`
2. **Por fila:** deserializa `imagenes_json`, reconstruye `AlternateView`, `EnviarEmailAsync`:
   - éxito → `enviado`, `enviado=now()`.
   - fallo → `intentos++`; si `>= max_intentos` → `fallido`; si no → `pendiente`, `proximo_intento = now() + backoff(intentos)`, `ultimo_error`.
3. **Backoff** `1m/5m/30m/2h/6h` como función pura/estática.
4. **Colgados:** fila en `enviando` > 15 min vuelve a `pendiente` (riesgo de duplicado raro asumido y documentado).

Registrado como `AddHostedService` junto a los otros dos workers.

## 4. Integración (los 4 puntos de envío)

| # | Dónde | Hoy | Pasa a |
|---|---|---|---|
| 1 | `ConsentimientosBL:264` creación | `Task.Run(… EnviarEmailAsync)` | `EncolarAsync(dbSigo, …)` en la tx de creación (se elimina el `Task.Run`) |
| 2 | `ConsentimientosBL:1185` reenvío | `await EnviarEmailAsync` | `EncolarAsync` `tipo=CONSENTIMIENTO_REENVIO` |
| 3 | `PasswordResetService` | envío sin `await` | `EncolarAsync` `tipo=PASSWORD_RESET`, `ref=usuario id` |
| 4 | `EncuestasBL`/`NotificacionEncuestasWorker` | envía directo | **encola** `tipo=ENCUESTA_NOTIF` |

Resultado: `NotificacionEncuestasWorker` decide **qué** notificar y encola; `EmailOutboxWorker` **envía**. `IEmailService` queda como transporte interno, único llamador = el worker; los BL solo dependen de `IEmailOutbox`.

Detalle a verificar al implementar: el path exacto de envío de encuestas (sitio 4).

## 5. Manejo de errores y verificación

**Errores:** worker indestructible (F7); fallos por fila aislados (`ultimo_error`); sin doble-envío (claim + `SKIP LOCKED`); colgados reciclados; SMTP mal configurado (depende de F1) → `fallido` visible en vez de pérdida silenciosa; encolado atómico (si falla el insert, falla la tx de negocio — prácticamente nunca).

**Verificación (manual + build; no hay test project):**
- `backoff(intento)` pura → verificable (y unit-testeable si agregan test project).
- E2E en VS + IIS Express `:44370`: (1) crear → `pendiente` → worker → `enviado`; (2) SMTP malo → reintenta → `fallido` tras 5; (3) reenvío/reset/encuesta → fila con su `tipo_origen`; (4) atomicidad: rollback de creación → sin fila.

**Fuera de alcance (YAGNI):** badge en `tyc-web`; adjuntos más allá del logo; rate-limit de correo por empresa; cambios de plantillas.

## Archivos (estimado)

- Nuevos: `Tyc.Modelo/Contexto/EmailOutbox.cs`, `Tyc.Interface/Services/IEmailOutbox.cs` (+ `EmailOutboxItem`), `Tyc.Implementacion/Email/EmailOutboxService.cs`, `FrameAppWS/Workers/EmailOutboxWorker.cs`, `scripts/email_outbox.sql`.
- Modificados: `ConsentimientosBL.cs` (sitios 1 y 2), `PasswordResetService.cs`, `EncuestasBL.cs`/`NotificacionEncuestasWorker.cs`, registro DI (`Program.cs`), y `TycWS.cs` (nuevo endpoint `GET /consentimientos/{guid}/estado-correo` de solo lectura).
