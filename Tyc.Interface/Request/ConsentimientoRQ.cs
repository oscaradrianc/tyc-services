using ServiceStack;
using System;
using System.Collections.Generic;
using Tyc.Interface.Response;
using Tyc.Modelo.Tipos;

namespace Tyc.Interface.Request
{
    [Route("/consentimientos/{Id}", "GET")]
    public class GetConsentimiento : IReturn<ConfirmacionConsentimientoRS>
    {
        public int Id { get; set; }
    }

    [Route("/consentimientos", "POST")]
    public class ConsentimientoRQ : IReturn<ApiResponse<int>>
    {
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public int? TipoIdentificacion { get; set; }
        public string? Identificacion { get; set; }
        public string? Medio { get; set; }
    }

    [Route("/consentimientos/{ConsentimientoId}/firma", "PUT")]
    public class ActualizarConsentimientoConFirma : IReturn<ApiResponse<bool>>
    {
        public int ConsentimientoId { get; set; }       
        public string FirmaImagen { get; set; }
        public List<PoliticaAceptadaItem> PoliticasAceptadas { get; set; }
        public List<string> OpcionesContactabilidad { get; set; }
        public DateTime FechaFirma { get; set; }
        public string Dispositivo { get; set; }
    }

    [Route("/consentimientos", "GET")]
    public class ListarConsentimientosRQ : IReturn<ApiResponse<List<ConsentimientoListItemRS>>>
    {
        /// <summary>
        /// Fecha de creación del consentimiento (filtro exacto por día)
        /// </summary>
        public DateTime? Fecha { get; set; }

        /// <summary>
        /// Estado: "F" = Firmado (tiene fecha aceptación), "P" = Pendiente (sin fecha aceptación)
        /// </summary>
        public string? Estado { get; set; }
    }
}
