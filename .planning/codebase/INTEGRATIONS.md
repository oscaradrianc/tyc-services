# External Integrations

**Analysis Date:** 2026-04-27

## APIs & External Services

**Email Delivery:**
- AWS SES (Simple Email Service) - Primary email provider via `AwsSesEmailService`
  - SDK: `AWSSDK.SimpleEmailV2` (v4.0.7)
  - Region configurable via `Email:AwsRegion`
  - Fallback to SMTP if `Email:Provider` != `"SMTP"`
- SMTP (AWS SES SMTP endpoint) - Fallback/default provider via `SmtpEmailService`
  - Host: `email-smtp.us-east-1.amazonaws.com`
  - Port: 587 with SSL
  - Credentials encrypted with AES-256 in `appsettings.json`

**reCAPTCHA:**
- Google reCAPTCHA v2/v3
  - Secret key in `appsettings.json`: `GoogleReCaptcha:SecretKey`
  - Used for public form validation

**WhatsApp (Twilio):**
- Twilio WhatsApp integration configured
  - `WPSid` (Account SID)
  - `WPToken` (Auth Token)
  - `WPFrom` (sender number: `+14155238886`)
  - Used for survey notifications

## Data Storage

**Databases:**
- Multi-database support via Devart.Data.Linq ORM:
  - **PostgreSQL** - Primary in production (`TycContextPostgreSQL`)
  - **Oracle** - Supported (`TycContextOracle`)
  - **SQL Server** - Supported (`TycContextSqlServer`)
- Two logical contexts:
  - `TycContext` (`TycBaseContext`) - Transactional data: consentimientos, empresas, firmas, encuestas
  - `TycAdmContext` (`SigoAdmBaseContext`) - Administrative data: enterprises, parameters, blobs
- Connection strings loaded from encrypted environment variable `CONNECTION_STRS_CONS_PROD`
- Devart license keys from `DEVART_KEYS` environment variable

**File Storage:**
- Local filesystem only
- Paths configured in `appsettings.json`:
  - `PathCargaArchivos`: `/app_data/`
  - `RootExportFiles`: `/app_data/`
  - `DirDest`: `/Temp/archPlantilla`
  - `PathCargaPlanos`: `/Temp`
- Email templates in `FrameAppWS/Templates/` (copied to output)
- Logo images in `FrameAppWS/Resources/`

**Caching:**
- Redis via `ServiceStack.Redis` (`PooledRedisClientManager`)
  - Connection config from `REDIS_CONFIG` environment variable
  - Used for session storage, rate limiting, distributed caching
- Local `MemoryCache` (`Microsoft.Extensions.Caching.Memory`) for in-process caching
  - Used for IP rate limiting in public endpoints

## Authentication & Identity

**Auth Provider:**
- JWT-based authentication via ServiceStack `[Authenticate]` attribute
- Token configuration in `appsettings.json`:
  - `jwt.AuthKeyBase64`: symmetric key
  - `secret`: additional secret string
  - `Issuer`: `https://sgsas.co`
  - `Audience`: `5igo`
  - `ExpireTokensIn`: 10 minutes
- `CustomUserSession` from `administradorcore.servicelogs` carries user identity, enterprise ID, connection strings
- Password reset flow implemented with encrypted tokens (`PasswordResetToken` entity)

## Monitoring & Observability

**Error Tracking:**
- Serilog with Elasticsearch sink
  - Index naming: `{app-name}-{environment}-{yyyy-MM-d}`
  - Buffered shipping with 10MB file buffer
  - Auto-register template for ESv7

**Logs:**
- Serilog structured logging with enrichment:
  - CorrelationId
  - Environment
  - ApplicationName / ApplicationVersion
  - ThreadId
- Console output with ANSI theme in development
- Elasticsearch sink in production (when `ELASTIC_HOST` is configured)
- File sink in development (`logs/frameWs.txt`)

**Health Checks:**
- `HealthCheckService` at `/health`, `/health/live`, `/health/ready`
- Checks Redis (ping) and Database (`SELECT 1`)
- Returns JSON with status, timestamp, version, and details

**Background Workers:**
- `MonitoringWorker` - Health monitoring (from administradorcore.basehost)
- `BloquearEmpresaWorker` - Daily at 6:00 AM, enterprise blocking logic
- `NotificacionEncuestasWorker` - Every 2 hours, survey notification scheduling

## CI/CD & Deployment

**Hosting:**
- Docker container deployment
- Base image: `mcr.microsoft.com/dotnet/aspnet:10.0`
- Port 8080 (Kestrel)
- Timezone: `America/Bogota`

**CI Pipeline:**
- GitHub Actions workflow in `.github/workflows/` (directory exists)
- Docker build with `NUGET_KEY` build arg for GitHub Packages

**Build Artifacts:**
- `App5igo` folder content copied to output (`PreserveNewest`)
- `app_data` excluded from compilation

## Infrastructure

**Web Server:**
- Kestrel (primary)
- IIS integration enabled (`builder.WebHost.UseIIS()`)
- Max request body size: `int.MaxValue`
- Form options configured for large multipart uploads

**Encryption:**
- `administradorcore.cifrar` (`BaseCifrado`) - AES-256 encryption
- Used for:
  - Sensitive personal data in database (names, emails, IDs, phones)
  - SMTP credentials in config
  - Password reset tokens
  - Consent form link GUIDs

**Rate Limiting:**
- Custom IP-based rate limiting in `PublicTycWS`
- MemoryCache-backed: 10 requests per minute per IP

## Environment Configuration

**Required Environment Variables:**
- `CONNECTION_STRS_CONS_PROD` - Encrypted connection string bundle
- `REDIS_CONFIG` - Encrypted Redis connection string
- `DEVART_KEYS` - Devart component license keys
- `ASPNETCORE_ENVIRONMENT` - Standard ASP.NET Core env

**Secrets in `appsettings.json` (encrypted values):**
- SMTP credentials (`SmtpUsuario`, `SmtpClave`)
- JWT keys (`jwt.AuthKeyBase64`, `secret`)
- VAPID keys for push notifications
- Twilio credentials
- reCAPTCHA secret key

**Sensitive config file:**
- `produccion.env` - Present in repo root, contains encrypted production settings

## Webhooks & Callbacks

**Incoming:**
- None detected

**Outgoing:**
- None detected (email is the primary outbound notification channel)

## Data Flow Diagram

```
Client (Frontend)
  -> ServiceStack WS (Tyc.Interface/*WS.cs)
    -> BL Service (Tyc.Implementacion/*BL.cs)
      -> Repository (Tyc.Implementacion/*/Repositories/)
        -> Devart LINQ DataContext (Tyc.Modelo/TycContext.cs)
          -> PostgreSQL / Oracle / SQL Server
```

Background workers bypass the WS layer and directly invoke BL services with manually constructed DataContext instances.

---

*Integration audit: 2026-04-27*
