using System;

namespace Tyc.Interface.Response
{
    public class ConfirmacionConsentimientoRS
    {
        public int Id { get; set; }
        public Guid? Guid { get; set; }
        public int IdEmpresa { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string TipoIdentificacion { get; set; }
        public string? Identificacion { get; set; }
        public DateTime? FechaAceptacion { get; set; }
        public string? Link { get; set; }
    }
}
