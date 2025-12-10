using System;

namespace Tyc.Interface.Response
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
        public string TipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaAceptacion { get; set; }
        public string Link { get; set; }
        public DateTime FechaCreacion { get;  set; }
        public string AceptoTerminos { get; set; }
        public string AceptoCompartirInformacion { get; set; }
        public string AceptoRecibirOfertas { get; set; }
        public string AceptoContatoTelefonico { get; set; }
        public string AceptoContatoEmail { get; set; }
        public string AceptoContatoSMS { get; set; }
        public string AceptoContatoWhatsApp { get; set; }
    }
}
