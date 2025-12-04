using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tyc.Interface.Services;

namespace Tyc.Implementacion.Email;

public class PlaceholderService : IPlaceholderService
{
    private readonly ILogger<PlaceholderService> _logger;

    // Regex para encontrar {{Placeholder}}
    private static readonly Regex PlaceholderRegex = new(
        @"\{\{(\w+)\}\}",
        RegexOptions.Compiled);

    // Mapeo de placeholder -> descripción (para documentación/UI)
    private static readonly Dictionary<string, string> PlaceholdersDisponibles = new()
    {
        // Datos del cliente
        { "NombreCliente", "Nombre del cliente" },
        { "ApellidoCliente", "Apellido del cliente" },
        { "NombreCompletoCliente", "Nombre y apellido del cliente" },
        { "EmailCliente", "Email del cliente" },
        { "MovilCliente", "Número móvil del cliente" },
        { "IdentificacionCliente", "Número de identificación" },
        { "TipoIdentificacionCliente", "Tipo de identificación (CC, NIT, etc.)" },
        { "FechaCreacion", "Fecha de creación del consentimiento" },
        
        // Datos de la empresa
        { "NombreEmpresa", "Nombre de la empresa/agencia" },
        { "TelefonoEmpresa", "Teléfono de contacto de la empresa" },
        { "EmailEmpresa", "Email de contacto de la empresa" },
        { "DireccionEmpresa", "Dirección de la empresa" },
        
        // Datos del formulario
        { "LinkFormulario", "URL del formulario de consentimiento" }
    };

    public PlaceholderService(ILogger<PlaceholderService> logger)
    {
        _logger = logger;
    }

    public string ReemplazarPlaceholders(string texto, PlaceholderData datos)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return texto;

        if (datos == null)
        {
            _logger.LogWarning("PlaceholderData es null, retornando texto sin cambios");
            return texto;
        }

        // Crear diccionario de valores
        var valores = CrearDiccionarioValores(datos);

        // Reemplazar usando Regex
        var resultado = PlaceholderRegex.Replace(texto, match =>
        {
            var placeholder = match.Groups[1].Value;

            if (valores.TryGetValue(placeholder, out var valor))
            {
                return valor ?? string.Empty;
            }

            // Si no encuentra el placeholder, lo deja como está y loguea warning
            _logger.LogWarning("Placeholder no reconocido: {{{{{Placeholder}}}}}", placeholder);
            return match.Value;
        });

        return resultado;
    }

    public Dictionary<string, string> ObtenerPlaceholdersDisponibles()
    {
        return new Dictionary<string, string>(PlaceholdersDisponibles);
    }

    private static Dictionary<string, string?> CrearDiccionarioValores(PlaceholderData datos)
    {
        return new Dictionary<string, string?>
        {
            // Datos del cliente
            { "NombreCliente", datos.Nombre },
            { "ApellidoCliente", datos.Apellido },
            { "NombreCompletoCliente", datos.NombreCompleto },
            { "EmailCliente", datos.Email },
            { "MovilCliente", datos.Movil },
            { "IdentificacionCliente", datos.Identificacion },
            { "TipoIdentificacionCliente", datos.TipoIdentificacion },
            { "FechaCreacion", datos.FechaCreacion?.ToString("dd/MM/yyyy HH:mm") },
            
            // Datos de la empresa
            { "NombreEmpresa", datos.NombreEmpresa },
            { "TelefonoEmpresa", datos.TelefonoEmpresa },
            { "EmailEmpresa", datos.EmailEmpresa },
            { "DireccionEmpresa", datos.DireccionEmpresa },
            
            // Datos del formulario
            { "LinkFormulario", datos.LinkFormulario }
        };
    }
}