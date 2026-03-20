# AGENTS.md - TycServices Development Guide

## Overview
This is a .NET 10.0 solution with 4 projects:
- **FrameAppWS** - Web API entry point (ASP.NET Core)
- **Tyc.Implementacion** - Business logic layer (BL classes, Repositories)
- **Tyc.Interface** - Interfaces, Request/Response DTOs
- **Tyc.Modelo** - Domain models and database entities

## Build Commands

```bash
# Build entire solution
dotnet build TycAppWS.sln

# Build specific project
dotnet build Tyc.Implementacion/Tyc.Implementacion.csproj

# Build in Release mode
dotnet build TycAppWS.sln -c Release

# Publish the web app
dotnet publish FrameAppWS/FrameAppWS.csproj -c Release -o ./publish
```

## Test Commands
**Note:** There are currently no test projects in this solution.

```bash
# If tests are added in the future:
dotnet test                    # Run all tests
dotnet test --filter "FullyQualifiedName~Namespace.TestClassName"  # Run single test
dotnet test --list-tests      # List available tests
```

## Code Style Guidelines

### Project Structure
```
Tyc.Modelo/
  Contexto/           # Database entities (ORM)
  Tipos/              # Custom types and DTOs
  Consultas/          # Query classes
  Configuracion/      # Configuration classes

Tyc.Interface/
  Services/           # Service interfaces (IxxxService)
  Repositories/        # Repository interfaces (IxxxRepository)
  Request/            # Request DTOs (*RQ)
  Response/           # Response DTOs (*RS)
                      # Subfolders: General/, Usuarios/, Consentimientos/, etc.

Tyc.Implementacion/
  [Feature]/
    xxxBL.cs          # Business logic class
    Repositories/     # Repository implementations
    Mappings/         # Mapster mapping configurations
```

### Naming Conventions
- **Interfaces**: Prefix with `I` (e.g., `IUsuarioService`)
- **Business Logic**: Suffix with `BL` (e.g., `UsuariosBL`)
- **Repositories**: Suffix with `Repository` (e.g., `UsuarioRepository`)
- **Request DTOs**: Suffix with `RQ` (e.g., `CambiarClaveRQ`)
- **Response DTOs**: Suffix with `RS` (e.g., `UsuarioRS`)
- **Database Entities**: PascalCase, descriptive (e.g., `Usuario`, `Consentimiento`)
- **Properties**: PascalCase (e.g., `UsuaLogin`, `UsuaEmail`)
- **Methods**: PascalCase, verb-oriented (e.g., `CrearUsuario`, `GetById`)

### Imports/Using Statements
Order imports alphabetically within groups:
```csharp
using System;
using System.Collections.Generic;
using Administrador.Core.Cifrar;
using Microsoft.Extensions.Logging;
using Tyc.Interface.Services;
using Tyc.Modelo;
```

### Formatting
- Use 4 spaces for indentation (no tabs)
- Opening braces on new line
- Maximum line length: 120 characters (soft limit)
- One blank line between namespace and class declaration
- Use expression-bodied members where appropriate

### Types
- Use explicit types (not `var`) for public API responses
- Use `var` for local variables when type is obvious
- Use nullable reference types (`string?`, `int?`) where appropriate
- Use `string.Empty` instead of `""`
- Use `nameof()` for parameter validation

### Error Handling
- Always wrap business logic in try-catch blocks
- Log errors with `_logger.LogError(ex, "message")`
- Return `ApiResponse<T>` with `Success = false` on errors
- Never expose internal exception details to clients
- Use specific exception types when applicable

Example:
```csharp
try
{
    var result = _repository.GetById(context, id);
    return new ApiResponse<UsuarioRS>
    {
        Success = true,
        Data = MapToDto(result)
    };
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error al obtener usuario {UsuarioId}", id);
    return new ApiResponse<UsuarioRS>
    {
        Success = false,
        Mensaje = "Ocurrió un error al consultar el usuario."
    };
}
```

### Dependency Injection
- Use constructor injection for all dependencies
- Register services in Program.cs using Scrutor assembly scanning
- Use appropriate lifetimes: `AddScoped` for repositories/services, `AddSingleton` for configurations

Example:
```csharp
public class UsuariosBL : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<UsuariosBL> _logger;

    public UsuariosBL(
        ILogger<UsuariosBL> logger,
        IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }
}
```

### Logging
- Use Serilog for structured logging
- Use log templates: `"Operation {Parameter} failed with {Error}"`
- Include correlation IDs for request tracing
- Log at appropriate levels: Debug, Information, Warning, Error

### Database Entities (Tyc.Modelo)
- Use Devart Data Linq attributes for ORM mapping
- Mark primary keys with `IsPrimaryKey = true`
- Use nullable types (`int?`, `DateTime?`) for optional fields
- Follow database column naming (e.g., `usua_usua`, `empr_empr`)

### API Responses
- Always return `ApiResponse<T>` or ServiceStack's `ApiResponse`
- Include meaningful `Mensaje` on both success and failure
- Set `Success` property explicitly

### Security
- Never log sensitive data (passwords, tokens, personal info)
- Use `BaseCifrado` for encryption/decryption
- Validate all inputs, especially from external sources
- Use parameterized queries (ORM handles this)

### Configuration
- Use `IOptions<T>` for configuration classes
- Store sensitive config in `appsettings.json` or secrets
- Use environment-specific settings (`appsettings.Development.json`)

### General Best Practices
- Keep classes focused (Single Responsibility Principle)
- Use dependency interfaces, not concrete implementations
- Avoid magic strings/numbers - use constants
- Document public APIs with XML comments
- Write code that's self-documenting
- Keep methods under 50 lines when possible

## Key Libraries/Frameworks
- **ServiceStack** - Web framework and ORM
- **Serilog** - Logging
- **Mapster** - Object mapping
- **Microsoft.Extensions.DependencyInjection** - DI container
- **Devart.Data.Linq** - LINQ to SQL (Oracle)
- **administradorcore** - Internal framework packages

## Additional Notes
- The solution uses central package management (`Directory.Packages.props`)
- Target framework: `net10.0`
- Uses User Secrets for development: `UserSecretsId=a0f6f205-78ce-46b6-ac1d-e6d82a3d9e83`
- Redis is used for caching
- AWS SES for email sending (with SMTP fallback)
