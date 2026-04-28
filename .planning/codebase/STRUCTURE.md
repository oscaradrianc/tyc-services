# Codebase Structure

**Analysis Date:** 2026-04-27

## Directory Layout

```
[project-root]/
├── FrameAppWS/                 # Entry point: ASP.NET host, DI, workers, templates
│   ├── Program.cs              # Application bootstrap
│   ├── HealthCheckService.cs   # Health/readiness endpoints
│   ├── AppLogsExt.cs           # Serilog/Elasticsearch logging config
│   ├── Workers/                # Background services
│   ├── Templates/              # HTML email templates
│   ├── Resources/              # Static images (logos)
│   └── appsettings*.json       # Configuration
├── Tyc.Interface/              # Contracts: DTOs, interfaces, web services
│   ├── *WS.cs                  # ServiceStack API endpoints
│   ├── Request/                # Input DTOs (RQ suffix)
│   ├── Response/               # Output DTOs (RS suffix)
│   ├── Services/               # Service interfaces (I*Service)
│   └── Repositories/           # Repository interfaces (I*Repository)
├── Tyc.Implementacion/         # Business logic, repositories, mappings
│   ├── Consentimientos/        # Consent domain
│   ├── Empresas/               # Enterprise domain
│   ├── Encuestas/              # Survey domain
│   ├── Firmas/                 # Signature domain
│   ├── Parametrizacion/        # Parameterization domain
│   ├── Pdf/                    # PDF generation service
│   ├── Textos/                 # Text/template domain
│   ├── Usuarios/               # User domain
│   └── Email/                  # Email services and template renderer
├── Tyc.Modelo/                 # Entities, contexts, types
│   ├── Contexto/               # Devart LINQ entities
│   │   ├── Administrador/      # Admin schema entities (tadm_*)
│   │   └── General/            # General schema entities (grl_*)
│   ├── Tipos/                  # Custom types, DTOs used internally
│   ├── Consultas/              # Query result types
│   ├── Configuracion/          # Configuration classes
│   ├── TycContext.cs           # Main DB context factory
│   └── TycAdmContext.cs        # Admin DB context factory
├── Directory.Packages.props    # Central package management
├── Directory.Build.props       # Shared build properties
├── Dockerfile                  # Docker multi-stage build
└── TycAppWS.sln               # Solution file
```

## Directory Purposes

**FrameAppWS:**
- Purpose: ASP.NET web host and application composition root
- Contains: Entry point, DI setup, background workers, email templates, health checks
- Key files: `FrameAppWS/Program.cs`, `FrameAppWS/Workers/BloquearEmpresaWorker.cs`, `FrameAppWS/HealthCheckService.cs`

**Tyc.Interface:**
- Purpose: API contracts and endpoint definitions
- Contains: ServiceStack service classes, request/response DTOs, service and repository interfaces
- Key files: `Tyc.Interface/TycWS.cs`, `Tyc.Interface/PublicTycWS.cs`, `Tyc.Interface/Request/ConsentimientoRQ.cs`

**Tyc.Implementacion:**
- Purpose: Business logic implementation and data access
- Contains: BL classes, repository implementations, Mapster mapping configs, email/PDF services
- Key files: `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs`, `Tyc.Implementacion/Pdf/PdfService.cs`

**Tyc.Modelo:**
- Purpose: Data entities and database context definitions
- Contains: Devart LINQ entity classes, context factories, custom types, constants
- Key files: `Tyc.Modelo/TycContext.cs`, `Tyc.Modelo/Contexto/Consentimiento.cs`, `Tyc.Modelo/ConstantesTyc.cs`

## Key File Locations

**Entry Points:**
- `FrameAppWS/Program.cs`: Application bootstrap, DI container, ServiceStack AppHost

**Configuration:**
- `FrameAppWS/appsettings.json`: Runtime configuration
- `FrameAppWS/appsettings.Development.json`: Development overrides
- `Directory.Packages.props`: Central NuGet package versions
- `Directory.Build.props`: Shared MSBuild properties

**Core Logic:**
- `Tyc.Implementacion/Consentimientos/ConsentimientosBL.cs`: Main consent business logic
- `Tyc.Implementacion/Empresas/EmpresasBL.cs`: Enterprise CRUD logic
- `Tyc.Implementacion/Textos/TextosBL.cs`: Text/template management
- `Tyc.Implementacion/Usuarios/UsuariosBL.cs`: User management and password
- `Tyc.Implementacion/Encuestas/EncuestasBL.cs`: Survey assignment and responses
- `Tyc.Implementacion/Pdf/PdfService.cs`: PDF generation

**API Endpoints:**
- `Tyc.Interface/PublicTycWS.cs`: Public endpoints (consent forms, password reset)
- `Tyc.Interface/TycWS.cs`: Authenticated consent operations
- `Tyc.Interface/EmpresasWS.cs`: Enterprise CRUD
- `Tyc.Interface/TextosWS.cs`: Text/template management
- `Tyc.Interface/UsuariosWS.cs`: User management
- `Tyc.Interface/EncuestasWS.cs`: Survey operations

**Testing:**
- No test projects exist yet

## Naming Conventions

**Files:**
- Web services: `{Domain}WS.cs` (e.g., `TycWS.cs`, `PublicTycWS.cs`)
- Business logic: `{Domain}BL.cs` (e.g., `ConsentimientosBL.cs`)
- Repositories: `{Domain}Repository.cs` (e.g., `ConsentimientoRepository.cs`)
- Request DTOs: suffix `RQ` (e.g., `ConsentimientoRQ`, `CreateEmpresa`)
- Response DTOs: suffix `RS` (e.g., `ConsentimientosRS`, `ApiResponse<T>`)
- Mapping configs: `{Domain}MappingConfig.cs` (e.g., `ConsentimientoMappingConfig.cs`)
- Interfaces: prefix `I` (e.g., `IConsentimientoService`, `IConsentimientoRepository`)

**Directories:**
- Domain folders use plural Spanish names: `Consentimientos/`, `Empresas/`, `Usuarios/`
- Each domain contains `Repositories/` and optionally `Mappings/` subfolders

**Database:**
- Table prefix: `tgen_` for main transactional tables, `tadm_` for admin tables, `grl_` for general tables
- Entity properties: PascalCase matching DB columns (e.g., `UsuaLogin`, `EmprEmpr`)

## Where to Add New Code

**New Feature (e.g., new domain):**
- Primary code: `Tyc.Implementacion/{Domain}/{Domain}BL.cs`
- Repository: `Tyc.Implementacion/{Domain}/Repositories/{Domain}Repository.cs`
- Interface: `Tyc.Interface/Services/I{Domain}Service.cs`
- Repository interface: `Tyc.Interface/Repositories/I{Domain}Repository.cs`
- Request DTOs: `Tyc.Interface/Request/{Domain}RQ.cs`
- Response DTOs: `Tyc.Interface/Response/{Domain}/`
- Web service: `Tyc.Interface/{Domain}WS.cs`
- Entity: `Tyc.Modelo/Contexto/{Entity}.cs`
- Mapping config: `Tyc.Implementacion/{Domain}/Mappings/{Domain}MappingConfig.cs`

**New API Endpoint:**
- Add request DTO to `Tyc.Interface/Request/`
- Add response DTO to `Tyc.Interface/Response/{Domain}/`
- Add method to existing `*WS.cs` or create new one
- Add method to service interface and BL implementation

**New Background Worker:**
- Implementation: `FrameAppWS/Workers/{Name}Worker.cs` extending `BackgroundService`
- Registration: `FrameAppWS/Program.cs` via `builder.Services.AddHostedService<>()`

**Utilities:**
- Shared helpers: `Tyc.Modelo/Tipos/` for data transfer types
- Cross-cutting services: `Tyc.Implementacion/Email/` for email-related utilities

## Special Directories

**FrameAppWS/Templates/:**
- Purpose: HTML email templates with placeholder syntax `[VariableName]`
- Files: `consentimiento-creado.html`, `consentimiento-firmado.html`, `mail-invitacion.html`, etc.
- Generated: No
- Committed: Yes

**FrameAppWS/Workers/:**
- Purpose: .NET `IHostedService` implementations for background tasks
- Generated: No
- Committed: Yes

**Tyc.Modelo/Contexto/Administrador/ and General/:**
- Purpose: Entities for external admin and general schemas
- Generated: No (hand-coded Devart LINQ entities)
- Committed: Yes

---

*Structure analysis: 2026-04-27*
