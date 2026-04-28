# Testing Patterns

**Analysis Date:** 2026-04-27

## Test Framework

**Runner:** Not detected. No test projects exist in the solution.

**Assertion Library:** Not applicable.

**Config:** Not applicable.

**Run Commands:**
```bash
# No test commands available - no test projects exist
dotnet test                    # Would fail - no test projects
dotnet test --watch           # Not applicable
dotnet test --collect:"XPlat Code Coverage"  # Not applicable
```

## Test File Organization

**Location:** No test files exist.

**Naming:** No test files exist.

**Structure:**
```
# Current solution structure (NO TESTS):
TycAppWS.sln
├── FrameAppWS/
├── Tyc.Implementacion/
├── Tyc.Interface/
└── Tyc.Modelo/

# Missing:
# ├── Tyc.Tests/          (unit tests)
# ├── Tyc.IntegrationTests/  (integration tests)
```

## Current State

**Zero test coverage.** The solution contains 4 projects and 150 C# source files with no corresponding test projects, test classes, or test methods.

**Projects in solution:**
- `FrameAppWS/FrameAppWS.csproj` - Entry point
- `Tyc.Implementacion/Tyc.Implementacion.csproj` - Business logic
- `Tyc.Interface/Tyc.Interface.csproj` - Contracts and endpoints
- `Tyc.Modelo/Tyc.Modelo.csproj` - Data entities and context

**Verification:**
```bash
# No files matching test patterns:
find . -type f \( -name "*.test.*" -o -name "*test*.cs" -o -name "*Test*.cs" -o -name "*.spec.*" \)
# (empty result)
```

## Test Gaps by Layer

**Web Service Layer (`Tyc.Interface`):**
- No endpoint tests for any ServiceStack services
- No auth flow tests for `[Authenticate]` attribute
- No request/response serialization tests
- Affected files: `TycWS.cs`, `PublicTycWS.cs`, `EmpresasWS.cs`, `UsuariosWS.cs`, `TextosWS.cs`, `EncuestasWS.cs`

**Business Logic Layer (`Tyc.Implementacion`):**
- No unit tests for `ConsentimientosBL` (1233 lines - largest file)
- No unit tests for `EmpresasBL`, `UsuariosBL`, `TextosBL`, `EncuestasBL`
- No unit tests for `PdfService` (847 lines)
- No unit tests for email services (`AwsSesEmailService`, `SmtpEmailService`)
- No tests for mapping configurations (`ConsentimientoMappingConfig`, `EmpresaMappingConfig`)

**Repository Layer (`Tyc.Implementacion/*/Repositories`):**
- No unit tests for `ConsentimientoRepository` (591 lines)
- No unit tests for `TextoRepository`, `EmpresaRepository`, `FirmaRepository`, `UsuarioRepository`, `EncuestaRepository`
- No tests for Devart LINQ query logic

**Background Workers (`FrameAppWS/Workers`):**
- No tests for `BloquearEmpresaWorker`
- No tests for `NotificacionEncuestasWorker`
- No tests for `MonitoringWorker`

## Recommended Testing Approach

**Framework:** xUnit (standard for .NET) or NUnit

**Mocking:** Moq or NSubstitute for repository/service mocking

**Test Project Structure:**
```
Tyc.Tests/
├── Unit/
│   ├── Consentimientos/
│   │   ├── ConsentimientosBLTests.cs
│   │   └── ConsentimientoRepositoryTests.cs
│   ├── Empresas/
│   ├── Usuarios/
│   ├── Textos/
│   ├── Encuestas/
│   └── Pdf/
├── Integration/
│   ├── ConsentimientoEndpointsTests.cs
│   └── PublicTycWSTests.cs
└── Tyc.Tests.csproj
```

**Key Patterns to Test:**

1. **Dual sync/async repository APIs** - Both `GetById` and `GetByIdAsync` should return identical results

2. **Task.Run wrapper pattern** - Verify async repository methods don't deadlock:
```csharp
// Pattern to test:
public async Task<Consentimiento> GetByIdAsync(TycBaseContext context, int id)
{
    return await Task.Run(() => context.GetTable<Consentimiento>()
        .FirstOrDefault(x => x.Id == id));
}
```

3. **ApiResponse<T> wrapper** - All WS methods return consistent response shape:
```csharp
// Assert all endpoints return:
// { Success: bool, Mensaje: string, Data: T }
```

4. **Validation throws** - BL methods throw correct exception types:
```csharp
// ConsentimientosBL validation patterns:
// - ArgumentException for invalid contactability options
// - InvalidOperationException for missing entities
// - InvalidOperationException for prefix validation failures
```

5. **Fire-and-forget email** - Background tasks don't crash the main flow:
```csharp
// _ = Task.Run(async () => { ... email logic ... })
// Should not affect return value even if email fails
```

6. **Mapster mappings** - Configuration classes in `*/Mappings/` folders:
```csharp
// ConsentimientoMappingConfig: ConsentimientoRQ -> Consentimiento
// EmpresaMappingConfig: CreateEmpresa -> Empresa
```

## Testing Challenges

**Devart.Data.Linq:**
- No in-memory provider available for unit testing
- Requires integration tests against real database or mocked `DataContext`
- `Task.Run(() => context.SubmitChanges())` pattern makes async behavior hard to unit test

**ServiceStack:**
- Integration tests require `AppHost` setup
- `SessionAs<CustomUserSession>()` requires mock session

**External Dependencies:**
- AWS SES client requires mocking
- Redis (`PooledRedisClientManager`) requires mocking or TestContainers
- Email templates loaded from filesystem

## Priority Test Additions

**High Priority:**
1. `ConsentimientosBL` business rules (existence validation, encryption, email flow)
2. `PdfService` PDF generation logic
3. `PublicTycWS` rate limiting and public endpoint validation
4. `TycWS` auth-protected endpoint flows

**Medium Priority:**
1. Repository query logic (filtering, joins)
2. Mapster mapping configurations
3. Email service error handling

**Low Priority:**
1. Background worker scheduling logic
2. Serilog configuration

## CI/CD Integration

**Current state:** No test execution in build pipeline.

**Recommended:**
```bash
# Add to build pipeline:
dotnet test Tyc.Tests/Tyc.Tests.csproj --no-build --verbosity normal
```

---

*Testing analysis: 2026-04-27*
