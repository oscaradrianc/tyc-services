using Microsoft.AspNetCore.Http;
using solg.lib.settings;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using static Tyc.Modelo.TycBaseContext;

namespace FrameAppWS.Middleware;

public class CorsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _baseDomain;

    public CorsMiddleware(RequestDelegate next, string baseDomain)
    {
        _next = next;
        _baseDomain = baseDomain;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var origin = context.Request.Headers["Origin"].ToString();

        if (!string.IsNullOrEmpty(origin))
        {
            bool permitido = ValidarOrigen(origin);

            if (permitido)
            {
                AgregarHeadersCors(context, origin);
            }

            // Manejar preflight OPTIONS
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                return;
            }
        }

        await _next(context);
    }

    private bool ValidarOrigen(string origin)
    {
        // 1. Localhost para desarrollo de Ionic/Angular
        if (EsDesarrolloLocal(origin))
            return true;

        // 2. Capacitor/Cordova para apps móviles
        if (EsAppMovilNativa(origin))
            return true;

        // 3. Producción: validar *.baseDomain
        if (origin.EndsWith(_baseDomain, StringComparison.OrdinalIgnoreCase))
            return ValidarSubdominioEnBD(origin);

        return false;
    }

    private bool EsDesarrolloLocal(string origin)
    {
        return origin.Contains("localhost") ||
               origin.Contains("127.0.0.1") ||
               origin.Contains("localhost:8100") ||  // Ionic serve
               origin.Contains("localhost:4200") ||  // Angular serve
               origin.Contains("localhost:4400");    // Angular backend
    }

    private bool EsAppMovilNativa(string origin)
    {
        return origin.StartsWith("capacitor://") ||
               origin.StartsWith("ionic://") ||
               origin.StartsWith("http://localhost") ||
               origin == "file://";
    }

    private bool ValidarSubdominioEnBD(string origin)
    {
        try
        {
            var uri = new Uri(origin);
            var host = uri.Host; // "age1.midominio.com"
            var subdominio = host.Replace($".{_baseDomain}", "");

            // Obtener configuración de conexión
            var settings = Settings.GetInstance();
            settings.SetDbConfig(true);
            string connectionString = settings.GetConnection("Tyc").connectionString;
            var motorBD = Administrador.Modelo.Contexto.MotorBD.POSTGRESQL;

            using (var dbContext = TycContext.DataContext(connectionString, motorBD))
            {
                return dbContext.GetTable<Empresa>()
                    .Any(a => a.Subdominio != null &&
                              a.Subdominio.Contains(subdominio));
            }
        }
        catch
        {
            return false;
        }
    }

    private void AgregarHeadersCors(HttpContext context, string origin)
    {
        context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers",
             "Content-Type, Authorization, X-Requested-With, Accept, Origin, " +
                 "app_adm, empr_empr, sist_sist, X-Custom-Header, Cache-Control, Pragma");
        
        context.Response.Headers.Append("Access-Control-Expose-Headers",
            "Content-Length, Content-Type, X-Custom-Header");

        context.Response.Headers.Append("Access-Control-Max-Age", "86400");
    }
}