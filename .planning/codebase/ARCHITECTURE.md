# Architecture

**Analysis Date:** 2026-04-27

## Pattern Overview

**Overall:** Layered Monolith

**Key Characteristics:**
- Four-project layered architecture with strict separation of concerns
- ServiceStack web framework for API endpoints (not ASP.NET MVC/WebAPI)
- Devart.Data.Linq ORM with multi-database support (PostgreSQL, Oracle, SQL Server)
- JWT-based authentication via ServiceStack `[Authenticate]` attribute
- Background workers using .NET `IHostedService` for scheduled tasks
- Synchronous and asynchronous method pairs in repositories and services

## Layers

**Presentation / API Layer (FrameAppWS):**
- Purpose: Application entry point, DI container setup, hosting configuration, background workers
- Location: `FrameAppWS/`
- Contains: `Program.cs`, `AppLogsExt.cs`, `HealthCheckService.cs`, `Workers/`
- Depends on: Tyc.Interface, Tyc.Implementacion
- Used by: External clients (web frontend, mobile)

**Interface / Contracts Layer (Tyc.Interface):**
- Purpose: API endpoint definitions (ServiceStack services), request/response DTOs, service and repository interfaces
- Location: `Tyc.Interface/`
- Contains: `*WS.cs` web services, `Request/`, `Response/`, `Services/`, `Repositories/`
- Depends on: Tyc.Modelo
- Used by: FrameAppWS, Tyc.Implementacion

**Business Logic Layer (Tyc.Implementacion):**
- Purpose: Business rules, orchestration, data transformation, external service integration
- Location: `Tyc.Implementacion/`
- Contains: `*BL.cs` business logic classes, `*/Repositories/`, `*/Mappings/`, `Email/`, `Pdf/`
- Depends on: Tyc.Interface, Tyc.Modelo
- Used by: FrameAppWS (via DI)

**Data Layer (Tyc.Modelo):**
- Purpose: Devart LINQ entities, database contexts, custom types, configuration classes
- Location: `Tyc.Modelo/`
- Contains: `Contexto/`, `Tipos/`, `Consultas/`, `Configuracion/`, `TycContext.cs`, `TycAdmContext.cs`
- Depends on: administradorcore.modelo (internal framework)
- Used by: All other layers

## Data Flow

**Authenticated Request Lifecycle:**

1. HTTP request arrives at ServiceStack `Service` subclass in `Tyc.Interface/*WS.cs`
2. WS extracts `CustomUserSession` from JWT token via `SessionAs<CustomUserSession>()`
3. WS instantiates `TycBaseContext` using `TycContext.DataContext(userSession)`
4. WS calls BL method, passing the context
5. BL calls Repository method(s), passing the context
6. Repository executes Devart LINQ queries via `context.GetTable<T>()`
7. BL applies business rules, encryption/decryption, email triggers
8. WS wraps result in `ApiResponse<T>` and returns to client

**Public Request Lifecycle (no auth):**

1. HTTP request arrives at `PublicTycWS` (no `[Authenticate]` attribute)
2. Rate limiting validated via `IMemoryCache`
3. Connection string resolved from `solg.lib.settings.Settings`
4. `TycContext.DataContext(connectionString, MotorBD.POSTGRESQL)` instantiated directly
5. Flow continues through BL -> Repository -> DB

**State Management:**
- No server-side session state for business data
- JWT tokens carry user identity (`CustomUserSession`)
- Redis used for distributed caching and ServiceStack session storage
- Local `MemoryCache` used for rate limiting on public endpoints

## Key Abstractions

**ApiResponse<T>:**
- Purpose: Uniform API response wrapper
- Location: `Tyc.Interface/Response/General/ApiResponse.cs`
- Pattern: All WS methods return `ApiResponse<T>` with `Success`, `Mensaje`, `Data`

**TycBaseContext:**
- Purpose: Abstract database context for Devart LINQ
- Location: `Tyc.Modelo/TycContext.cs`
- Pattern: Factory method `TycContext.DataContext()` selects provider (PostgreSQL/Oracle/SQL Server) at runtime

**IRegister (Mapster):**
- Purpose: Object mapping configuration
- Examples: `Tyc.Implementacion/Consentimientos/Mappings/ConsentimientoMappingConfig.cs`
- Pattern: Auto-scanned by Scrutor in `Program.cs`

**IEmailService:**
- Purpose: Pluggable email provider
- Implementations: `SmtpEmailService`, `AwsSesEmailService`
- Selection: Config-driven via `Email:Provider` setting in `Program.cs`

**IConsentimientoService / IConsentimientoRepository:**
- Purpose: Core domain abstraction for consent management
- Pattern: Service interface in `Tyc.Interface/Services/`, Repository interface in `Tyc.Interface/Repositories/`

## Entry Points

**Web Application:**
- Location: `FrameAppWS/Program.cs`
- Triggers: Kestrel/IIS HTTP requests
- Responsibilities: DI registration, Mapster config, ServiceStack AppHost, worker registration, Redis setup

**Background Workers:**
- `BloquearEmpresaWorker` (`FrameAppWS/Workers/BloquearEmpresaWorker.cs`): Daily at 6:00 AM, blocks enterprises based on survey deadlines
- `NotificacionEncuestasWorker` (`FrameAppWS/Workers/NotificacionEncuestasWorker.cs`): Every 2 hours, sends pending survey notifications
- `MonitoringWorker` (from administradorcore.basehost): Health monitoring

**Health Checks:**
- Location: `FrameAppWS/HealthCheckService.cs`
- Endpoints: `/health`, `/health/live`, `/health/ready`
- Checks: Redis connectivity, database connectivity

## Error Handling

**Strategy:** Try-catch in BL classes with structured logging

**Patterns:**
- BL catches exceptions, logs with `_logger.LogError(ex, "message")`, returns `ApiResponse<T>` with `Success = false`
- WS throws `HttpError` subclasses (`NotFound`, `BadRequest`, `Validation`, `Unauthorized`) for HTTP-specific errors
- Public endpoints catch and wrap in controlled response objects (e.g., `FormularioConsentimientoRS` with `EsValido = false`)

## Cross-Cutting Concerns

**Logging:** Serilog with Elasticsearch sink, enriched with correlation ID, thread ID, environment. Filtered to exclude health checks and GET requests.

**Validation:** Business validation in BL classes (e.g., `ValidarEmpresa`, `ValidarOpcionesContactabilidad`). ServiceStack route validation via DTO attributes.

**Authentication:** JWT tokens via ServiceStack `[Authenticate]` attribute. `CustomUserSession` from administradorcore.servicelogs. Public endpoints explicitly omit the attribute.

**Encryption:** AES-256 via `BaseCifrado` from administradorcore.cifrar. Per-enterprise key using `EmpresaId`. Sensitive fields encrypted at rest.

**Email:** Template-based HTML emails with inline images. Templates in `FrameAppWS/Templates/`. Placeholder replacement via `SimpleTemplateRenderer`.

**PDF Generation:** QuestPDF for consent form PDFs. Single-document and bulk period reports.

---

*Architecture analysis: 2026-04-27*
