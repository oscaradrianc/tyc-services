using System;

namespace Tyc.Interface.Response.Consentimientos
{
    public class ConsentimientosRS
    {
        public string Empresa { get;set;}
        public Guid Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Identificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }           
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaAceptacion { get; set; }
        public string Medio { get; set; }
        public string Estado { get; set; }
        public string TipoPersona { get; set; }
        public string RazonSocial { get; set; }
        public string NombreContacto { get; set; }
        public string Referencia { get; set; }
        public string AceptoTYC { get; set; }
        public string AceptoCompartirInfo { get; set; }
        public string AceptoRecibirOfertas { get; set; }
        public string ContactabilidadSms { get; set; }
        public string ContactabilidadWhatsapp { get; set; }
        public string ContactabilidadEmail { get; set; }
        public string ContactabilidadMovil { get; set; }
        public string AceptoTerminosPersonaJuridica { get; set; }

    }
}
