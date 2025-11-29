using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.Net;
using Tyc.Interface.Request;
using Tyc.Interface.Response;
using Tyc.Interface.Services;
using Tyc.Modelo;
using static Tyc.Modelo.TycBaseContext;

namespace Tyc.Interface;

// SIN atributo [Authenticate] - servicio público
public class PublicTycWS : Service
{
    private readonly IConsentimientoService _consentimientoService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PublicTycWS> _logger;

    public PublicTycWS(
        IConsentimientoService consentimientoService,
        IMemoryCache cache,
        ILogger<PublicTycWS> logger)
    {
        _consentimientoService = consentimientoService;
        _cache = cache;
        _logger = logger;
    }

    public ApiResponse<FormularioConsentimientoRS> Get(ObtenerFormularioConsentimiento request)
    {
        string clientIp = Request.UserHostAddress;        

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
        string connectionString = settings.GetConnection("Tyc").connectionString;
        var motorBD = Administrador.Modelo.Contexto.MotorBD.POSTGRESQL;

        using (var dbContext = TycContext.DataContext(connectionString, motorBD))
        {
            try
            {
                var result = _consentimientoService.ObtenerFormularioConsentimiento(
                    dbContext,
                    request.Subdominio,
                    request.Id
                );

                // 3. Log de acceso exitoso
                _logger.LogInformation($"Formulario accedido - IP: {clientIp}, Consent: {result.Consentimiento.Id}");

                return new ApiResponse<FormularioConsentimientoRS>
                {
                    Data = result,
                    Mensaje = "Formulario obtenido exitosamente",
                    Success = true
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
}