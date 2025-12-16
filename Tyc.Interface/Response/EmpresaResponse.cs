#nullable enable
using System;

namespace Tyc.Interface.Response;

public class EmpresaResponse
{
    public int Id { get; set; }
    public Guid? Guid { get; set; }

    // Información básica
    public string? Nombre { get; set; }
    public string? CiudadEmpresa { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Website { get; set; }
    public string? MailContactos { get; set; }
    public string? LogoEmpresa { get; set; }

    // Certificaciones
    public byte[]? LogoIso9000 { get; set; }
    public byte[]? LogoIso27001 { get; set; }

    // Contacto
    public string? NombreContacto { get; set; }
    public string? MailDelContacto { get; set; }
    public string? TelContacto { get; set; }
    public string? Subdominio { get; set; }

    // Configuración de términos
    public string? ManejaTerminosYCondiciones { get; set; }
    public string? ManejaTycCompartirInfo { get; set; }
    public string? ManejaTycRecibirOfertas { get; set; }

    // Configuración de contactabilidad
    public string? ContactabilidadSms { get; set; }
    public string? ContactabilidadEmail { get; set; }
    public string? ContactabilidadWhatsapp { get; set; }
    public string? ContactabilidadMovil { get; set; }

    // Configuración de campos solicitados
    public string? SolicitaNombre { get; set; }
    public string? SolicitaApellido { get; set; }
    public string? SolicitaEmail { get; set; }
    public string? SolicitaTelefono { get; set; }
    public string? SolicitaIdentificacion { get; set; }

    // Estado
    public string? Estado { get; set; }

    // Tipos de negocio
    public string? ManejaCorporativo { get; set; }
    public string? ManejaConsolidacion { get; set; }
    public string? ManejaReceptivo { get; set; }
    public string? ManejaMayoreo { get; set; }
    public string? ManejaEventos { get; set; }
}