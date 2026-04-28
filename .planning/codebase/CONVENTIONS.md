# Coding Conventions

**Analysis Date:** 2026-04-27

## Naming Patterns

**Files:**
- Business Logic: `XxxBL.cs` (e.g., `ConsentimientosBL.cs`, `EmpresasBL.cs`)
- Repositories: `XxxRepository.cs` (e.g., `ConsentimientoRepository.cs`)
- Web Service endpoints: `XxxWS.cs` (e.g., `TycWS.cs`, `EmpresasWS.cs`)
- Request DTOs: suffix `RQ` (e.g., `ConsentimientoRQ.cs`, `GetEmpresa`)
- Response DTOs: suffix `RS` (e.g., `ConsentimientosRS.cs`, `UsuarioRS`)
- Mapping configs: `XxxMappingConfig.cs` (e.g., `ConsentimientoMappingConfig.cs`)
- Background workers: suffix `Worker` (e.g., `BloquearEmpresaWorker.cs`)

**Interfaces:**
- Services: `IXxxService` (e.g., `IConsentimientoService`, `IEmpresaService`)
- Repositories: `IXxxRepository` (e.g., `IConsentimientoRepository`, `ITextoRepository`)

**Functions:**
- Async methods: suffix `Async` (e.g., `ObtenerConfirmacionConsentimientoAsync`, `CrearConsentimientoAsync`)
- Public methods: PascalCase (e.g., `ObtenerEmpresaPorId`, `ActualizarAceptaciones`)
- Private methods: PascalCase (e.g., `ValidarOpcionesContactabilidad`, `CifrarDatosSensibles`)

**Variables:**
- Private fields: underscore prefix + camelCase (e.g., `_repository`, `_logger`, `_mapper`)
- Local variables: camelCase with `var` when type is obvious
- Constants: PascalCase or ALL_CAPS in static classes (e.g., `ConstantesTyc.llaveParametroLink`)

**Types:**
- Classes: PascalCase (e.g., `ConsentimientosBL`, `ApiResponse<T>`)
- DB entity properties: PascalCase matching DB columns (e.g., `UsuaLogin`, `EmprEmpr`, `ConsNombre`)
- Enums: PascalCase members (e.g., `OpcionSiNo.Si`, `OpcionSiNo.No`)

## Code Style

**Formatting:**
- 4 spaces indentation
- Opening braces on new lines (Allman style)
- Use `var` for local variables when type is obvious from right-hand side
- Explicit types for public API returns and complex expressions

**Example:**
```csharp
public async Task<StatusResult> ActualizarConsentimientoConFirmaAsync(
    TycBaseContext context, 
    ActualizarConsentimientoConFirma request)
{
    var consentimientoExistente = await _repository.GetByGuidAsync(context, request.ConsentimientoId);

    if (consentimientoExistente == null)
    {
        throw new InvalidOperationException($"Consentimiento {request.ConsentimientoId} no encontrado");
    }
    // ...
}
```

**Linting:**
- No `.editorconfig` or StyleCop ruleset detected at solution root
- No enforced linting configuration

## Import Organization

**Order:**
1. `System.*` namespaces
2. Third-party libraries (ServiceStack, Mapster, Serilog, etc.)
3. Project references (`Tyc.Interface.*`, `Tyc.Modelo.*`, `Tyc.Implementacion.*`)

**Within each group:** Alphabetical ordering

**Example from `ConsentimientosBL.cs`:**
```csharp
using AdministradorCore.Cifrar;
using AngleSharp.Dom;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Tyc.Interface.Repositories;
using Tyc.Interface.Request;
using Tyc.Interface.Response.Consentimientos;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using Tyc.Modelo.Tipos;
using static Tyc.Interface.Request.ConsentimientoPublicoRQ;
using Empresa = Tyc.Modelo.Contexto.Empresa;
```

**Path Aliases:**
- Type aliases used for disambiguation: `using Empresa = Tyc.Modelo.Contexto.Empresa;`
- Static imports for constants: `using static Tyc.Interface.Request.ConsentimientoPublicoRQ;`

## Error Handling

**Patterns:**

**Business Logic Layer (`Tyc.Implementacion`):**
- Try-catch blocks wrapping business operations
- Return `ApiResponse<T>` with `Success = false` on errors
- Log exceptions with structured templates via `ILogger<T>`
- Throw `ArgumentException` or `InvalidOperationException` for validation failures

**Example from `UsuariosBL.cs`:**
```csharp
try
{
    // ... business logic ...
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error al crear el usuario.");
    return new ApiResponse<UsuarioRS>
    {
        Success = false,
        Mensaje = "Ocurrió un error al crear el usuario."
    };
}
```

**Web Service Layer (`Tyc.Interface`):**
- Throw `HttpError.NotFound()` for missing resources
- Throw `HttpError.BadRequest()` for invalid parameters
- Throw `HttpError.Validation()` for business validation failures
- Throw `HttpError.Unauthorized()` for auth failures

**Example from `TycWS.cs`:**
```csharp
if (result == null)
    throw HttpError.NotFound($"Consentimiento {request.Id} no encontrado");
```

**Repository Layer:**
- Minimal error handling; exceptions bubble up to BL
- Return `false` or `null` for not-found scenarios

## Logging

**Framework:** Serilog + Microsoft.Extensions.Logging (`ILogger<T>`)

**Patterns:**
- Structured logging with named placeholders (NOT string interpolation)
- Use `_logger.LogError(ex, "message {Param}", value)` format

**Examples:**
```csharp
_logger.LogWarning("No se encontró empresa {EmpresaId}", entity.EmpresaId);
_logger.LogInformation("Email enviado exitosamente a {Destinatario}. MessageId: {MessageId}", destinatario, response.MessageId);
_logger.LogError(ex, "Error al enviar correo de confirmación para consentimiento {Id}", request.ConsentimientoId);
```

**Log Levels:**
- `LogInformation` - successful operations
- `LogWarning` - business-level issues (missing data, rate limits)
- `LogError` - exceptions and failures
- `LogCritical` - fatal worker errors

## Async/Await Patterns

**Async Method Naming:**
- All async methods use `Async` suffix
- Both sync and async versions exist in repositories (dual API)

**Devart LINQ Async Wrapper:**
- Devart.Data.Linq does not natively support async; wrap in `Task.Run()`
- Pattern used throughout repositories (39 occurrences)

**Example from `ConsentimientoRepository.cs`:**
```csharp
public async Task<Consentimiento> GetByGuidAsync(TycBaseContext context, Guid guid)
{
    return await Task.Run(() => context.GetTable<Consentimiento>()
        .FirstOrDefault(x => x.GuId == guid));
}
```

**Fire-and-Forget:**
- Background email sending uses `_ = Task.Run(async () => { ... })`
- Always wrapped in try-catch to prevent unobserved exceptions

**Example from `ConsentimientosBL.cs`:**
```csharp
_ = Task.Run(async () =>
{
    try
    {
        bool enviado = await _emailService.EnviarEmailAsync(...);
        // ...
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al enviar email...");
    }
});
```

## Comments

**When to Comment:**
- XML doc comments on public methods (`<summary>`, `<param>`, `<returns>`)
- Section headers for logical blocks within long methods
- Inline comments for business rules or non-obvious logic

**Example:**
```csharp
/// <summary>
/// Reemplaza placeholders {{Variable}} en el texto con los valores del diccionario.
/// </summary>
/// <param name="plantilla">Texto con placeholders. Ej: "Hola {{NombreCliente}}"</param>
public string ProcesarPlantillaTexto(string plantilla, Dictionary<string, string> variables)
```

**Commented Code:**
- Significant blocks of commented-out code exist in `UsuariosBL.cs` and `TextosBL.cs`
- Should be removed or extracted to version control

## Function Design

**Size:**
- Large BL methods are common (e.g., `CrearConsentimientoAsync` ~200 lines)
- Private helper methods extracted for validation (`ValidarOpcionesContactabilidad`, `ValidarYProcesarPoliticasAsync`)

**Parameters:**
- `TycBaseContext` passed as first parameter to repository and BL methods
- Request DTOs for complex inputs
- Optional parameters with defaults: `bool forceInsert = false`

**Return Values:**
- `ApiResponse<T>` for WS layer returns
- `StatusResult` for boolean status with message
- Tuple returns for composite results: `(Guid? Id, ConsentimientoExistenteRS Existente)`

## Module Design

**Exports:**
- One public class per file
- Interfaces in `Tyc.Interface` project, implementations in `Tyc.Implementacion`

**Barrel Files:**
- Not used; imports reference specific namespaces

**Dependency Injection:**
- Constructor injection throughout
- Explicit `AddScoped` registrations in `Program.cs`
- Scrutor assembly scanning for `IRegister` implementations (Mapster configs)

**Example registration from `Program.cs`:**
```csharp
builder.Services.AddScoped<IConsentimientoRepository, ConsentimientoRepository>();
builder.Services.AddScoped<IConsentimientoService, ConsentimientosBL>();
```

## Special Patterns

**ReadOnlySpan Parsing:**
```csharp
ReadOnlySpan<char> concatenado = usuarioId.ToString().AsSpan();
ReadOnlySpan<char> prefijo = empresaId.ToString().AsSpan();
if (!concatenado.StartsWith(prefijo))
    throw new InvalidOperationException("Prefijo inválido");
int idUsuario = int.Parse(concatenado[prefijo.Length..]);
```

**DB Context Lifecycle:**
- `using (TycBaseContext dbSigo = TycContext.DataContext(userSession))` in WS layer
- Context passed down through BL to repositories

---

*Convention analysis: 2026-04-27*
