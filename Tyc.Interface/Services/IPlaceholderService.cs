using System;
using System.Collections.Generic;

namespace Tyc.Interface.Services;

/// <summary>
/// Servicio para reemplazar placeholders en textos dinámicos.
/// Los placeholders usan formato {{NombrePlaceholder}}
/// </summary>
public interface IPlaceholderService
{
    /// <summary>
    /// Reemplaza los placeholders en el texto con los valores del consentimiento.
    /// </summary>
    /// <param name="texto">Texto con placeholders ej: "Hola {{NombreCliente}}"</param>
    /// <param name="datos">Datos para el reemplazo</param>
    /// <returns>Texto con los placeholders reemplazados</returns>
    string ReemplazarPlaceholders(string texto, PlaceholderData datos);

    /// <summary>
    /// Obtiene la lista de placeholders disponibles con su descripción.
    /// Útil para mostrar al usuario qué variables puede usar.
    /// </summary>
    Dictionary<string, string> ObtenerPlaceholdersDisponibles();
}

/// <summary>
/// Datos disponibles para reemplazo de placeholders
/// </summary>
public class PlaceholderData
{
    // Datos del cliente (ya descifrados)
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? NombreCompleto => $"{Nombre} {Apellido}".Trim();
    public string? Email { get; set; }
    public string? Movil { get; set; }
    public string? Identificacion { get; set; }
    public string? TipoIdentificacion { get; set; }
    public DateTime? FechaCreacion { get; set; }

    // Datos de la empresa
    public string? NombreEmpresa { get; set; }
    public string? TelefonoEmpresa { get; set; }
    public string? EmailEmpresa { get; set; }
    public string? DireccionEmpresa { get; set; }

    // Datos del formulario
    public string? LinkFormulario { get; set; }
}