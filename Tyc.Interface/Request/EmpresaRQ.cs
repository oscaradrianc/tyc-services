using ServiceStack;
using Tyc.Interface.Response;

namespace Tyc.Interface.Request;

[Route("/empresas/{Id}", "GET")]
public class GetEmpresa : IReturn<EmpresaResponse>
{
    public int Id { get; set; }
}

[Route("/empresas", "POST")]
public class CreateEmpresa : IReturn<EmpresaResponse>
{
    // Información básica
    public string Nombre { get; set; }
    public string CiudadEmpresa { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string Website { get; set; }
    public string MailContactos { get; set; }
    public string LogoEmpresa { get; set; }

	// Certificaciones
	public string TieneIso9000 { get; set; }
    public string TieneIso27001 { get; set; }

    // Contacto
    public string NombreContacto { get; set; }
    public string MailDelContacto { get; set; }
    public string TelContacto { get; set; }
    public string Subdominio { get; set; }

    // Configuración de términos
    public string ManejaTerminosYCondiciones { get; set; }
    public string ManejaTycCompartirInfo { get; set; }
    public string ManejaTycRecibirOfertas { get; set; }

    // Configuración de contactabilidad
    public string ContactabilidadSms { get; set; }
    public string ContactabilidadEmail { get; set; }
    public string ContactabilidadWhatsapp { get; set; }
    public string ContactabilidadMovil { get; set; }

    // Configuración de campos solicitados
    public string SolicitaNombre { get; set; }
    public string SolicitaApellido { get; set; }
    public string SolicitaEmail { get; set; }
    public string SolicitaTelefono { get; set; }
    public string SolicitaIdentificacion { get; set; }

    // Tipos de negocio
    public string ManejaCorporativo { get; set; }
    public string ManejaConsolidacion { get; set; }
    public string ManejaReceptivo { get; set; }
    public string ManejaMayoreo { get; set; }
    public string ManejaEventos { get; set; }
}

[Route("/Empresas/{Id}", "PUT")]
public class UpdateEmpresa : IReturn<EmpresaResponse>
{
    public int Id { get; set; }

    // Mismos campos que CreateEmpresa
    public string Nombre { get; set; }
    public string CiudadEmpresa { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string Website { get; set; }
    public string MailContactos { get; set; }
    public string LogoEmpresa { get; set; }
	public string LogoIso9000 { get; set; }
    public string LogoIso27001 { get; set; }
    public string NombreContacto { get; set; }
    public string MailDelContacto { get; set; }
    public string TelContacto { get; set; }
    public string Subdominio { get; set; }
    public string ManejaTerminosYCondiciones { get; set; }
    public string ManejaTycCompartirInfo { get; set; }
    public string ManejaTycRecibirOfertas { get; set; }
    public string ContactabilidadSms { get; set; }
    public string ContactabilidadEmail { get; set; }
    public string ContactabilidadWhatsapp { get; set; }
    public string ContactabilidadMovil { get; set; }
    public string SolicitaNombre { get; set; }
    public string SolicitaApellido { get; set; }
    public string SolicitaEmail { get; set; }
    public string SolicitaTelefono { get; set; }
    public string SolicitaIdentificacion { get; set; }
    public string ManejaCorporativo { get; set; }
    public string ManejaConsolidacion { get; set; }
    public string ManejaReceptivo { get; set; }
    public string ManejaMayoreo { get; set; }
    public string ManejaEventos { get; set; }
}