# Codebase Concerns

**Analysis Date:** 2026-04-27

## Tech Debt

### Duplicate sync/async repository implementations
- Issue: Every repository maintains both synchronous and asynchronous method pairs (e.g., `GetById` / `GetByIdAsync`, `Create` / `CreateAsync`, `Update` / `UpdateAsync`). The async variants wrap sync calls in `Task.Run(() => ...)`, which does not provide true async I/O and wastes thread-pool threads.
- Files: `Tyc.Implementacion/Consentimientos/Repositories/ConsentimientoRepository.cs`, `Tyc.Implementacion/Empresas/Repositories/EmpresaRepository.cs`, `Tyc.Implementacion/Textos/Repositories/TextoRepository.cs`, `Tyc.Implementacion/Firmas/Repositories/FirmaRepository.cs`, `Tyc.Implementacion/Usuarios/Repositories/UsuarioRepository.cs`
- Impact: Code bloat, higher memory/CPU usage under load, misleading API surface.
- Fix approach: Remove sync variants and implement true async Devart LINQ operations, or at minimum keep only the async surface and remove the `Task.Run` wrappers.

### Commented-out code blocks
- Issue: Large blocks of commented code remain in source files (old password-change logic, old text validation, SQL injection checks).
- Files: `Tyc.Implementacion/Usuarios/UsuariosBL.cs` (lines 116-374), `Tyc.Implementacion/Textos/TextosBL.cs` (lines 90-211), `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs` (lines 793-804)
- Impact: Noise, misleading future maintainers, potential security logic that looks active but is not.
- Fix approach: Delete commented code; rely on git history if needed.

### Empty stub class
- Issue: `ParametrizacionBL` is an empty internal class with no implementation.
- File: `Tyc.Implementacion/Parametrizacion/ParametrizacionBL.cs`
- Impact: Dead code, misleading project structure.
- Fix approach: Implement or remove.

### Unused OleDb / FoxPro dependency
- Issue: `System.Data.OleDb` package and `CadenaOLE` VFPOLEDB connection string in `appsettings.json` suggest legacy FoxPro integration that may no longer be used.
- Files: `Directory.Packages.props`, `FrameAppWS/appsettings.json`
- Impact: Unnecessary dependency, potential attack surface.
- Fix approach: Verify usage and remove if dead.

## Security Concerns

### Secrets committed in appsettings.json
- Issue: `appsettings.json` contains hardcoded secrets: JWT signing key (`jwt.AuthKeyBase64`), `secret`, VAPID keys (`VapidPublic`/`VapidPrivate`), Twilio credentials (`WPSid`, `WPToken`, `WPFrom`), SMTP credentials (encrypted but present), reCAPTCHA secret key, and Elasticsearch credentials in `appsettings.Development.json`.
- Files: `FrameAppWS/appsettings.json`, `FrameAppWS/appsettings.Development.json`
- Impact: Secrets are in git history; anyone with repo access can decrypt SMTP credentials using the known `ConstantesTyc.llaveParametroLink` key.
- Current mitigation: SMTP values are AES-encrypted with `BaseCifrado`, but the key (`"aF8S-Z"`) is a hardcoded constant in `ConstantesTyc.cs`.
- Recommendations: Rotate all secrets immediately. Move secrets to environment variables or a vault (Azure Key Vault, AWS Secrets Manager). Remove `appsettings.Development.json` from source control.

### Weak encryption key for link parameters
- Issue: `ConstantesTyc.llaveParametroLink = "aF8S-Z"` is a short, hardcoded string used to encrypt/decrypt consentimiento GUID links.
- File: `Tyc.Modelo/ConstantesTyc.cs`
- Impact: Brute-forceable key; an attacker who guesses the key can forge or decrypt consentimiento links.
- Recommendations: Use a per-deployment secret from configuration, enforce minimum key length (32 bytes for AES-256), and rotate periodically.

### SQL injection check is disabled and naive
- Issue: `UsuariosBL.SqlCheckInjection` contains a blacklist-based check that is commented out in `CambiarClave`. The blacklist approach is ineffective (easy to bypass with encoding, comments, etc.).
- File: `Tyc.Implementacion/Usuarios/UsuariosBL.cs`
- Impact: If re-enabled, it gives a false sense of security. If left disabled, there is no parameterized query validation visible in the password-reset flow.
- Recommendations: Remove the method entirely; rely on parameterized queries/ORM which Devart LINQ provides.

### Missing input validation on public endpoints
- Issue: `PublicTycWS` endpoints validate only emptiness (`string.IsNullOrWhiteSpace`). No length limits, no format validation on `Subdominio` or `Id`. The `Id` parameter is decrypted but not bounded in size before decryption.
- File: `Tyc.Interface/PublicTycWS.cs`
- Impact: Potential DoS via large payloads, unexpected exceptions leaking stack traces in development mode (`UseDeveloperExceptionPage`).
- Recommendations: Add `MaxLength`/`StringLength` validators, use ServiceStack validation attributes, and ensure production does not expose exception pages.

### Rate limit uses in-process MemoryCache
- Issue: `ValidarRateLimit` in `PublicTycWS` uses `IMemoryCache` with a 1-minute sliding window. This is per-instance and does not work behind a load balancer.
- File: `Tyc.Interface/PublicTycWS.cs`
- Impact: An attacker can bypass rate limiting by hitting different instances.
- Recommendations: Use Redis-backed rate limiting or an API gateway.

### Unbounded request size
- Issue: `FormOptions` and `IISServerOptions` are configured with `int.MaxValue` for body and multipart limits.
- File: `FrameAppWS/Program.cs` (lines 107-117)
- Impact: DoS risk via large upload requests exhausting memory.
- Recommendations: Set reasonable limits based on actual maximum expected payload (e.g., 10-50 MB).

### Email content injection via template variables
- Issue: `SimpleTemplateRenderer.RenderTemplate` does string replacement of `[Key]` with raw values. If user-controlled data (e.g., `NombreCliente`) contains HTML/JS, it is injected into the email body.
- File: `Tyc.Implementacion/Email/SimpleTemplateRenderer.cs`
- Impact: HTML injection in emails, potential phishing vectors.
- Recommendations: HTML-encode all substituted values before rendering.

### AWS SES email body is empty
- Issue: `AwsSesEmailService.EnviarEmailAsync` constructs a `SendEmailRequest` with only a subject; the `Body` is commented out and `AlternateView` parameter is ignored.
- File: `Tyc.Implementacion/Email/AwsSesEmailService.cs`
- Impact: Emails sent via AWS SES have no body; this is a silent functional failure.
- Recommendations: Implement body serialization from the `AlternateView` parameter.

## Performance Issues

### Fake async (`Task.Run`) on all repository operations
- Issue: Every repository async method wraps synchronous Devart LINQ calls in `Task.Run`. This blocks thread-pool threads and provides no scalability benefit.
- Files: All repository files under `Tyc.Implementacion/*/Repositories/`
- Impact: Under load, thread-pool starvation and degraded throughput.
- Improvement path: Use Devart's async APIs if available, or batch operations and accept synchronous I/O within background workers only.

### N+1 queries in listing endpoints
- Issue: `ConsentimientosBL.ListarConsentimientosPorEmpresaAsync` iterates consentimientos and calls `GetTipoIdentificacionAsync` per item inside a `foreach` loop.
- File: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs` (lines 991-1040)
- Impact: One query per consentimiento for type identification; severe latency with large datasets.
- Improvement path: Batch-load `TipoIdentificacion` data using `GetTiposIdentificacionByIdsAsync` (pattern already exists in `PdfService.CargarDatosRelacionadosAsync`).

### In-memory filtering after DB fetch
- Issue: `ListarConsentimientosPorEmpresaAsync` fetches all records for a company/date range, then applies `terminoBusqueda` filtering in memory with `resultado.Where(...).ToList()`.
- File: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs` (lines 1042-1054)
- Impact: High memory usage and latency when datasets are large; pagination is missing.
- Improvement path: Push search filters to the database query or add server-side pagination.

### PDF generation loads all consentimientos into memory
- Issue: `PdfService.GenerarConsentimientosPorPeriodoPdfAsync` loads up to 500 consentimientos and all related data into memory before generating a single PDF.
- File: `Tyc.Implementacion/Pdf/PdfService.cs`
- Impact: Large temporary memory spike; 500-record cap is arbitrary and may still OOM with large images.
- Improvement path: Stream PDF generation or paginate output into multiple files.

### Background workers create new DataContext per tick without connection pooling verification
- Issue: `BloquearEmpresaWorker` and `NotificacionEncuestasWorker` instantiate `TycContext.DataContext` inside each timer tick. Devart monitors are enabled (`IsActive = true`), which can add overhead.
- Files: `FrameAppWS/Workers/BloquearEmpresaWorker.cs`, `FrameAppWS/Workers/NotificacionEncuestasWorker.cs`
- Impact: Potential connection leaks if `using` blocks are missed; monitor overhead in production.
- Improvement path: Verify connection pooling is configured; disable monitors in production.

## Maintainability Problems

### Giant BL class (ConsentimientosBL)
- Issue: `ConsentimientosBL` is 1,233 lines, handling creation, update, listing, PDF coordination, email orchestration, encryption, and confirmation logic.
- File: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs`
- Impact: Violates Single Responsibility Principle; difficult to unit test, review, or modify safely.
- Safe modification: Extract email orchestration into a dedicated service, extract encryption into a domain service, and split listing logic from mutation logic.

### Tight coupling to `BaseCifrado` throughout BL
- Issue: `BaseCifrado` is instantiated inline in many methods with hardcoded keys. This makes encryption behavior impossible to mock or configure per environment.
- Files: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs`, `Tyc.Implementacion/Usuarios/UsuariosBL.cs`, `Tyc.Implementacion/Pdf/PdfService.cs`
- Impact: Cannot run integration tests with fake encryption; cannot rotate keys without redeploying code.
- Safe modification: Introduce an `IDataEncryptionService` interface and inject it.

### Hardcoded link generation URL
- Issue: The consentimiento form link is built with a hardcoded scheme and domain pattern: `$"https://{empresa?.Subdominio}.consentimiento.co?id=..."`.
- File: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs` (line 265)
- Impact: Cannot deploy to staging or custom domains without code changes.
- Safe modification: Move base URL to configuration (`IConfiguration` or `IOptions`).

### Duplicate `CifradoHelper` class
- Issue: `CifradoHelper` is defined as a private nested class in both `ConsentimientosBL` and `PdfService` with identical logic.
- Files: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs`, `Tyc.Implementacion/Pdf/PdfService.cs`
- Impact: Violates DRY; fixes must be applied in multiple places.
- Safe modification: Extract to a shared domain utility.

### User ID concatenation hack
- Issue: `usuarioId` is concatenated with `empresaId` as a string prefix (`empresaId || usua_usua`) and then parsed back with `ReadOnlySpan<char>` logic in multiple places.
- Files: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs`, `Tyc.Implementacion/Encuestas/EncuestasBL.cs`, `Tyc.Implementacion/Textos/TextosBL.cs`, `Tyc.Implementacion/Usuarios/UsuariosBL.cs`
- Impact: Fragile, error-prone, no compile-time safety. If the prefix logic changes, all parsing sites break.
- Safe modification: Use a composite key DTO or pass both IDs explicitly through the service layer.

## Missing Practices

### No automated tests
- Issue: Zero test projects exist in the solution. No unit tests, no integration tests, no E2E tests.
- Impact: Regressions are only caught in manual or production testing.
- Fix approach: Add an xUnit/NUnit test project targeting `Tyc.Implementacion`. Start with BL unit tests using in-memory fakes for repositories.

### No CI/CD pipeline
- Issue: `.github/workflows/` directory exists but is empty. No build, test, or deployment automation.
- Impact: Manual builds, no enforced quality gates, risk of shipping broken code.
- Fix approach: Add a GitHub Actions workflow for `dotnet build`, `dotnet test`, and Docker image push.

### No API documentation / OpenAPI
- Issue: ServiceStack is used but there is no visible OpenAPI/Swagger configuration or XML documentation generation.
- Impact: Frontend teams and integrators lack discoverable API docs.
- Fix approach: Enable ServiceStack's OpenAPI feature or add Swashbuckle.

### No structured request validation
- Issue: DTOs use ServiceStack routes but lack `[Validate]` attributes or FluentValidation. Validation is manual (`string.IsNullOrWhiteSpace`, `throw ArgumentException`) scattered in BL and WS layers.
- Impact: Inconsistent validation, duplicated logic, harder to maintain.
- Fix approach: Introduce FluentValidation or ServiceStack validation attributes on request DTOs.

## Dependency Risks

### Internal framework on beta versions
- Issue: `administradorcore.*` packages are pinned to `4.0.4-beta9`. These are internal packages from a private GitHub Packages feed.
- Files: `Directory.Packages.props`
- Impact: Beta packages may have unstable APIs or bugs. Tight coupling to internal framework makes migration hard.
- Migration plan: Track stable releases of internal framework; abstract framework-specific logic behind local interfaces to reduce coupling.

### DevExpress private feed dependency
- Issue: DevExpress packages (`DevExpress.Document.Processor.es`, etc.) are pinned to `[25.1.6]` and fetched from a private NuGet feed with an embedded key in the Dockerfile.
- Files: `Directory.Packages.props`, `Dockerfile`
- Impact: Build breaks if the private feed key expires or the feed is unreachable. License compliance risk.
- Migration plan: Cache DevExpress packages in an internal artifact repository; verify license allows containerized builds.

### Devart.Data.Linq (LINQ to SQL alternative)
- Issue: `Devart.Data.Linq` 5.3.0 is a commercial, niche ORM. It lacks community support, documentation, and true async support compared to EF Core or Dapper.
- Files: `Directory.Packages.props`
- Impact: Hiring difficulty, limited async capabilities, vendor lock-in.
- Migration plan: Evaluate migration to EF Core with existing PostgreSQL/Oracle/SQL Server support, or Dapper for query-heavy paths.

### AWSSDK.Core version mismatch
- Issue: `AWSSDK.Core` is pinned to `3.7.300.12` while `AWSSDK.SimpleEmailV2` is `4.0.7`. The v4 AWS SDK packages typically depend on `AWSSDK.Core` 3.7.x, but mixing major versions across the AWS SDK surface can cause runtime issues.
- Files: `Directory.Packages.props`
- Impact: Potential runtime binding redirects or missing method exceptions.
- Fix approach: Align all AWSSDK packages to compatible versions.

## Scalability Concerns

### Stateful in-process rate limiting
- Issue: `PublicTycWS.ValidarRateLimit` uses `IMemoryCache` which is local to the process.
- Impact: Does not scale horizontally; rate limits are per-instance, not global.
- Scaling path: Replace with Redis-backed rate limiting or move rate limiting to the reverse proxy/API gateway.

### Background workers run on every instance
- Issue: `BloquearEmpresaWorker` and `NotificacionEncuestasWorker` are `IHostedService` registered in every running container. With multiple replicas, all instances will attempt to run the same scheduled work simultaneously.
- Files: `FrameAppWS/Program.cs` (lines 119-121)
- Impact: Duplicate notifications, duplicate blocking attempts, race conditions.
- Scaling path: Use a distributed job scheduler (Hangfire with PostgreSQL storage, Quartz.NET with clustering) or leader-election via Redis.

### No caching on hot paths
- Issue: `TextoRepository.GetByEmpresaAsync`, `EmpresaRepository.GetByIdAsync`, and `TipoIdentificacion` lookups are uncached. These are fetched repeatedly for every consentimiento operation.
- Impact: Unnecessary database load for mostly static data (company config, text templates).
- Scaling path: Add Redis or MemoryCache caching with short TTL for empresa and texto configurations.

### Synchronous email sending in fire-and-forget
- Issue: `ConsentimientosBL.CrearConsentimientoAsync` uses `_ = Task.Run(async () => { ... await _emailService.EnviarEmailAsync(...) })` to send emails in the background.
- File: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs` (lines 270-318)
- Impact: Unobserved exceptions crash the process; no retry logic; if the app restarts, pending emails are lost.
- Scaling path: Use an outbox pattern with a background worker, or integrate a message queue (RabbitMQ, SQS) for reliable email delivery.

## Error Handling Gaps

### Swallowed exceptions in email confirmation
- Issue: `ActualizarConsentimientoConFirmaAsync` catches the email confirmation exception, logs it, and does not rethrow. The caller receives `Success = true` even though the confirmation email failed.
- File: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs` (lines 391-403)
- Impact: Silent failures; users and admins are unaware that the confirmation email was not sent.
- Fix approach: Return a partial-success response or queue the email for retry.

### Generic error messages hide root cause
- Issue: Most BL catch blocks return `"Ocurrió un error al ..."` without including any correlation ID or actionable detail.
- Files: `Tyc.Implementacion/Usuarios/UsuariosBL.cs`, `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs`
- Impact: Harder to debug production issues; support teams cannot trace failures.
- Fix approach: Return a correlation ID in `ApiResponse<T>` and log the full exception with structured logging.

### `catch (Exception)` in repositories without logging
- Issue: `EncuestaRepository.CrearAsignacionConDetalles` and `GuardarRespuestasCliente` catch `Exception`, roll back, and rethrow without logging.
- File: `Tyc.Implementacion/Encuestas/Repositories/EncuestaRepository.cs`
- Impact: Transaction rollback reason is lost if the exception is swallowed higher up.
- Fix approach: Log the exception before rollback/rethrow, or remove the try/catch and let the BL layer handle logging.

### `CifradoHelper.Descifrar` silently returns raw value on failure
- Issue: If decryption throws, the method returns the original (encrypted) string instead of failing.
- File: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs` (lines 750-763)
- Impact: Data corruption downstream (encrypted strings treated as plaintext); silent security degradation.
- Fix approach: Throw a specific `DecryptionException` and let callers handle it explicitly.

## Other Red Flags

### Build errors in current branch
- Issue: `build2.txt` shows compilation errors in `TextosWS.cs` (`ITextoService` missing methods `ObtenerTextoPorId` and `ObtenerTextosPorEmpresa`). The interface `ITextoService` only has async variants, but `TextosWS.cs` calls sync variants.
- File: `Tyc.Interface/TextosWS.cs` (lines 38, 55)
- Impact: The solution does not compile in its current state.
- Fix approach: Update `TextosWS` to call the async methods (`ObtenerTextoPorIdAsync`, `ObtenerTextosPorEmpresaAsync`) or add sync methods to the interface.

### `ParametrizacionBL` is empty but folder structure exists
- Issue: `Tyc.Implementacion/Parametrizacion/Mappings/` and `Repositories/` folders are empty placeholders.
- File: `Tyc.Implementacion/Tyc.Implementacion.csproj`
- Impact: Cluttered project structure.
- Fix approach: Remove empty folders or implement the planned feature.

### `Middleware` folder is empty in FrameAppWS
- Issue: `FrameAppWS.csproj` contains an empty `Middleware` folder.
- Impact: Minor clutter.
- Fix approach: Remove if unused.

### Docker image targets .NET 10 preview/runtime
- Issue: `Dockerfile` uses `mcr.microsoft.com/dotnet/sdk:10.0` and `aspnet:10.0`. .NET 10 is not an LTS release at the time of analysis.
- Impact: Potential instability, shorter support lifecycle.
- Fix approach: Evaluate pinning to the latest stable LTS (e.g., .NET 8) or ensure .NET 10 GA is targeted.

### Health check does not verify actual DB connectivity
- Issue: `HealthCheckService.Get(HealthCheckRQ)` returns `"Healthy"` without checking any dependency. The readiness check uses `HostContext.AppHost.GetDbConnection()` which may use a different connection path than the application's Devart contexts.
- File: `FrameAppWS/HealthCheckService.cs`
- Impact: False positives during deployments; Kubernetes may route traffic to an unhealthy pod.
- Fix approach: Use the same connection factory/settings as the app to execute a lightweight query.

---

*Concerns audit: 2026-04-27*
