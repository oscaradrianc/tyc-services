using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Response.Consentimientos;

public class FormularioConsentimientoRS
{
    public ConfigEmpresaData Config { get; set; }
    public ConsentimientoData Consentimiento { get; set; }
    public List<TextoData> Textos { get; set; }
    
    // Campos para manejo de errores controlados
    public bool EsValido { get; set; } = true;
    public string MensajeError { get; set; }
    public string CodigoError { get; set; }
}

public class ConfigEmpresaData
{
    public string Nombre { get; set; }
    public string CiudadEmpresa { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string Website { get; set; }
    public string MailContactos { get; set; }
    public string LogoEmpresa { get; set; }
    public byte[] LogoIso9000 { get; set; }
    public byte[] LogoIso27001 { get; set; }
    public string ManejaTerminosYCondiciones { get; set; }
    public string ManejaTycCompartirInfo { get; set; }
    public string ManejaTycRecibirOfertas { get; set; }
    public string ManejaTerminosPersonaJuridica { get; set; }
    public string ContactabilidadSms { get; set; }
    public string ContactabilidadEmail { get; set; }
    public string ContactabilidadWhatsapp { get; set; }
    public string ContactabilidadMovil { get; set; }
    public string SolicitaNombre { get; set; }
    public string SolicitaApellido { get; set; }
    public string SolicitaEmail { get; set; }
    public string SolicitaTelefono { get; set; }
    public string SolicitaIdentificacion { get; set; }
    public List<TipoIdentificacionData> TiposIdentificacion { get; set; }
    
}

public class ConsentimientoData
{
    public int Id { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public int? TipoIdentificacion { get; set; }
    public string Identificacion { get; set; }
    public string Estado { get; set; }
    public string TipoPersona { get; set; }
    public string RazonSocial { get; set; }
    public string NombreContacto { get; set; }
    public string Referencia { get; set; }
}

public class TextoData
{
    public int Id { get; set; }   
    public string TipoTexto { get; set; }
    public string TextoTerminos { get; set; }
}

public class TipoIdentificacionData
{
    public int Id { get; set; }
    public string Nombre { get; set; }
}