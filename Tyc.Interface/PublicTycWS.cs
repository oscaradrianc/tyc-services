using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.Net;
using System.Threading.Tasks;
using Tyc.Interface.Request;
using Tyc.Interface.Response.Consentimientos;
using Tyc.Interface.Response.General;
using Tyc.Interface.Services;
using Tyc.Modelo;
using static Tyc.Interface.Request.ConsentimientoPublicoRQ;

namespace Tyc.Interface;

// SIN atributo [Authenticate] - servicio público
public class PublicTycWS : Service
{
    private readonly IConsentimientoService _consentimientoService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PublicTycWS> _logger;
    private readonly IPasswordResetService _passwordResetService;

    public PublicTycWS(
        IConsentimientoService consentimientoService,
        IMemoryCache cache,
        ILogger<PublicTycWS> logger,
        IPasswordResetService passwordResetService)
    {
        _consentimientoService = consentimientoService;
        _cache = cache;
        _logger = logger;
        _passwordResetService = passwordResetService;
    }

    public async Task<ApiResponse<FormularioConsentimientoRS>> Get(ObtenerFormularioConsentimiento request)
    {
        string clientIp = string.Empty;

        if (IPAddress.TryParse(Request.UserHostAddress, out var ip))
        {
            clientIp = ip.IsIPv4MappedToIPv6
                ? ip.MapToIPv4().ToString()
                : ip.ToString();
        }

        if (!ValidarRateLimit(clientIp))
        {
            throw new HttpError(HttpStatusCode.TooManyRequests,
                    "TooManyRequests",
                    "Demasiados intentos. Intente en 1 minuto.");
        }

        // 2. Validación básica
        if (string.IsNullOrWhiteSpace(request.Subdominio) || string.IsNullOrWhiteSpace(request.Id))
            throw HttpError.BadRequest("Parámetros inválidos");

        var settings = solg.lib.settings.Settings.GetInstance();
        settings.SetDbConfig(true);

        //var connectionString = settings.GetAppSetting.GetConnection("SIGO"); 
        string connectionString = settings.GetConnection("Consentimiento").connectionString;
        var motorBD = Administrador.Modelo.Contexto.MotorBD.POSTGRESQL;

        using (var dbContext = TycContext.DataContext(connectionString, motorBD))
        {
            try
            {
                var result = await _consentimientoService.ObtenerFormularioConsentimientoAsync(
                    dbContext,
                    request.Subdominio,
                    request.Id
                );

                return new ApiResponse<FormularioConsentimientoRS>
                {
                    Data = result,
                    Mensaje = result.MensajeError,
                    Success = result.EsValido
                };
            }
            catch (Exception ex)
            {
                // 4. Log de intentos fallidos
                _logger.LogWarning($"Intento fallido - IP: {clientIp}, Error: {ex.Message}");
                throw;
            }
        }
    }

    private bool ValidarRateLimit(string ip)
    {
        string cacheKey = $"ratelimit_{ip}";

        if (!_cache.TryGetValue(cacheKey, out int requestCount))
            requestCount = 0;

        if (requestCount >= 10)
        {
            _logger.LogWarning($"Rate limit excedido - IP: {ip}, Intentos: {requestCount}");
            return false;
        }

        _cache.Set(cacheKey, requestCount + 1, TimeSpan.FromMinutes(1));
        return true;
    }

    public async Task<ApiResponse<bool>> Put(ActualizarConsentimiento request)
    {
        string clientIp = string.Empty;

        if (IPAddress.TryParse(Request.UserHostAddress, out var ip))
        {
            clientIp = ip.IsIPv4MappedToIPv6
                ? ip.MapToIPv4().ToString()
                : ip.ToString();
        }

        if (!ValidarRateLimit(clientIp))
        {
            throw new HttpError(HttpStatusCode.TooManyRequests,
                    "TooManyRequests",
                    "Demasiados intentos. Intente en 1 minuto.");
        }

        // 2. Validación básica
        if (string.IsNullOrWhiteSpace(request.Subdominio) || string.IsNullOrWhiteSpace(request.Id))
            throw HttpError.BadRequest("Parámetros inválidos");

        var settings = solg.lib.settings.Settings.GetInstance();
        settings.SetDbConfig(true);

        //var connectionString = settings.GetAppSetting.GetConnection("SIGO"); 
        string connectionString = settings.GetConnection("Consentimiento").connectionString;
        var motorBD = Administrador.Modelo.Contexto.MotorBD.POSTGRESQL;

        using (var dbContext = TycContext.DataContext(connectionString, motorBD))
        {
            try
            {
                request.IpClienteFirma = clientIp;

                var res = await _consentimientoService.ActualizarConsentimientoAsync(dbContext, request);

                if (!res.Status)
                {
                    throw HttpError.Validation("Validación Consentimiento", $"No se pudo actualizar el consentimiento", string.Empty);
                }

                return new ApiResponse<bool>
                {
                    Success = res.Status,
                    Mensaje = res.Message
                };
            }
            catch (Exception ex)
            {
                // 4. Log de intentos fallidos
                _logger.LogWarning($"Intento fallido - IP: {clientIp}, Error: {ex.Message}");
                throw;
            }
        }
    }

    

    // Agregar al constructor existente
    // IPasswordResetService passwordResetService

    public object Post(ForgotPasswordRQ request)
    {
        var settings = solg.lib.settings.Settings.GetInstance();
        settings.SetDbConfig(true);

        string connectionString = settings.GetConnection("Consentimiento").connectionString;
        var motorBD = Administrador.Modelo.Contexto.MotorBD.POSTGRESQL;

        using (var dbContext = TycContext.DataContext(connectionString, motorBD))
        {
            var frontendUrl = settings.GetAppSetting("urlBackend");
            _passwordResetService.GenerateResetToken(dbContext, request.UsuaLogin, request.Email, frontendUrl);

            // Siempre respuesta genérica

            return new ApiResponse<bool> { 
                Success = true, 
                Mensaje = "Si el login está registrado y el correo esta asociado al login concuerda, recibirás un enlace.",
                Data = true
            };
        }
    }

    public object Post(ResetPasswordRQ request)
    {
        var settings = solg.lib.settings.Settings.GetInstance();
        settings.SetDbConfig(true);
        string connectionString = settings.GetConnection("Consentimiento").connectionString;
        var motorBD = Administrador.Modelo.Contexto.MotorBD.POSTGRESQL;

        using (var dbContext = TycContext.DataContext(connectionString, motorBD))
        { 
            _passwordResetService.ResetPassword(dbContext, request.Token, request.Email, request.NewPassword);
            return new { Message = "Contraseña actualizada correctamente." };
        }
    }
}