using System;

namespace Tyc.Interface.Response.Consentimientos
{
    public class ConfirmacionConsentimientoRS
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int IdEmpresa { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int? IdTipoIdentificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaAceptacion { get; set; }
        public string Link { get; set; }
        public DateTime FechaCreacion { get;  set; }
        public string AceptoTerminos { get; set; }
        public int? IdTextoAceptoTerminos { get; set; }
        public string AceptoCompartirInformacion { get; set; }
        public int? IdTextoAceptoCompartirInformacion { get; set; }
        public string AceptoRecibirOfertas { get; set; }
        public int? IdTextoAceptoRecibirOfertas { get; set; }
        public string AceptoTerminosPersonaJuridica { get; set; }
        public int? IdTextoAceptoTerminosPersonaJuridica { get; set; }
        public string AceptoContatoTelefonico { get; set; }
        public string AceptoContatoEmail { get; set; }
        public string AceptoContatoSMS { get; set; }
        public string AceptoContatoWhatsApp { get; set; }
        public string TipoPersona { get; set; }
        public string RazonSocial { get; set; }
        public string NombreContacto { get; set; }
        public string Referencia { get; set; }
        public string IpFirma { get; set; }
        public string FuncionCreo { get; set; }
        public string MedioFirmo { get; set; }
        public byte[] FirmaImagen { get; set; }

        /// <summary>Texto CONSENTIMIENTO de la empresa con las variables {{...}} ya resueltas. Null si no lo tiene configurado.</summary>
        public string TextoConsentimiento { get; set; }

        /// <summary>Texto TEXTO_CONFIRMACION de la empresa con las variables {{...}} ya resueltas. Null si no lo tiene configurado.</summary>
        public string TextoConfirmacion { get; set; }
    }
}
