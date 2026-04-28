# Technology Stack

**Analysis Date:** 2026-04-27

## Languages

**Primary:**
- C# 12 / .NET 10.0 - All server-side code, web services, business logic, data access

**Secondary:**
- HTML - Email templates in `FrameAppWS/Templates/`
- JavaScript - Not used (backend-only API)

## Runtime

**Environment:**
- .NET 10.0 (`net10.0`) - Target framework across all projects
- ASP.NET Core 10.0 - Web host in `FrameAppWS`

**Package Manager:**
- NuGet with Central Package Management (`Directory.Packages.props`)
- Lockfile: Not present
- Private feeds:
  - GitHub Packages: `https://nuget.pkg.github.com/SolucionesGlobalesSAS/index.json` (administradorcore packages)
  - DevExpress private feed: `https://nuget.devexpress.com/...` (document processing)

## Frameworks

**Core:**
- ServiceStack 10.0.6 - Web framework, routing, DI, caching, Redis, ORM-lite helpers
- ServiceStack.Server 10.0.6 - Server-side utilities
- ServiceStack.Redis 10.0.6 - Redis client

**Data Access:**
- Devart.Data.Linq 5.3.0 - LINQ ORM supporting PostgreSQL, Oracle, SQL Server

**Object Mapping:**
- Mapster 7.4.0 - DTO/entity mapping
- Mapster.DependencyInjection 1.0.1 - DI integration

**PDF Generation:**
- QuestPDF 2025.12.4 - PDF generation (Community license)
- DevExpress.Document.Processor.es 25.1.6 - Document processing (Spanish)
- DevExpress.Drawing.Skia.es 25.1.6 - Skia drawing
- DevExpress.Pdf.SkiaRenderer.es 25.1.6 - PDF rendering

**HTML Processing:**
- HtmlAgilityPack 1.12.4 - HTML parsing
- HtmlSanitizer 9.0.892 - HTML sanitization
- Stubble.Core 1.10.8 - Mustache template engine

**Templating:**
- Stubble.Core 1.10.8 - Lightweight Mustache renderer used for email templates

**DI & Scanning:**
- Scrutor 7.0.0 - Assembly scanning for DI registration

**Logging:**
- Serilog 4.2.0 - Structured logging
- Serilog.Sinks.ElasticSearch 9.0.3 - Elasticsearch sink
- Serilog.Sinks.Console 6.1.1 - Console sink
- Serilog.Settings.Configuration 9.0.0 - Configuration binding
- Serilog.Extensions.Logging 9.0.0 - Microsoft.Extensions.Logging integration
- Serilog.Enrichers.CorrelationId 3.0.1
- Serilog.Enrichers.Environment 3.0.1
- Serilog.Enrichers.Process 3.0.0
- Serilog.Enrichers.Thread 3.1.0
- Serilog.Enrichers.HttpContext 8.0.9
- ServiceStack.Logging.Serilog 10.0.6 - ServiceStack adapter for Serilog

**AWS SDK:**
- AWSSDK.SimpleEmail 4.0.2.5 - SES v1
- AWSSDK.SimpleEmailV2 4.0.7 - SES v2
- AWSSDK.Core 3.7.300.12

**Other Libraries:**
- DocumentFormat.OpenXml 3.2.0 - OpenXML document processing
- System.Data.OleDb 9.0.1 - OLE DB data access
- System.CodeDom 9.0.1 - Code DOM
- TimeZoneConverter 7.2.0 - Timezone handling

**Internal Framework (administradorcore):**
- administradorcore.basehost 4.0.4-beta9 - Base host, AppHostFramework, logging setup
- administradorcore.cifrar 4.0.4-beta9 - AES-256 encryption (`BaseCifrado`)
- administradorcore.implementacion 4.0.4-beta9 - Shared implementations
- administradorcore.modelo 4.0.4-beta9 - Shared models
- administradorcore.servicelogs 4.0.4-beta9 - Service logging, auth (`CustomUserSession`)
- utilidadescore 4.0.4-beta9 - Utilities (`Settings`, `solg.lib.settings`)
- solg.lib.geo 1.0.0 - Geo utilities

## Build System

**Build Tool:**
- MSBuild via `dotnet` CLI
- Solution file: `TycAppWS.sln`
- Central Package Management enabled (`Directory.Packages.props`)

**Build Configuration:**
- `Directory.Build.props` sets:
  - `TargetFramework`: `net10.0`
  - `Version`: `2026.0804.05`
  - `Company`: `Soluciones Globales SAS`
  - `ManagePackageVersionsCentrally`: `true`

**Docker:**
- Multi-stage Dockerfile using `mcr.microsoft.com/dotnet/sdk:10.0` and `mcr.microsoft.com/dotnet/aspnet:10.0`
- Port 8080 exposed
- Timezone: `America/Bogota`

## Frontend Tech

**None.** This is a backend API-only project. No JavaScript framework, no CSS framework, no SPA.
- HTML email templates only in `FrameAppWS/Templates/`
- API consumed by external frontend(s)

## Dev Tools

**IDE:**
- Visual Studio (`.vs` directory present)

**Version Control:**
- Git (`.git` directory present)

**No explicit linting/formatting tools detected:**
- No `.editorconfig` found
- No StyleCop or similar analyzers referenced
- Code style governed by team conventions (see CONVENTIONS.md)

**Logging/Monitoring Dev Tools:**
- Serilog with Elasticsearch sink for centralized logging
- Health check endpoint at `/health` and `/health/ready`

## Platform Requirements

**Development:**
- .NET 10.0 SDK
- Docker (optional, for containerized builds)
- Access to private NuGet feeds (GitHub Packages token, DevExpress key)

**Production:**
- Docker container on Linux
- Kestrel web server (port 8080)
- IIS support configured (`builder.WebHost.UseIIS()`)
- Environment variables for connection strings and Redis config

---

*Stack analysis: 2026-04-27*
