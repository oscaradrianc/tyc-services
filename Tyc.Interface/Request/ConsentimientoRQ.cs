using ServiceStack;
using System;
using System.Collections.Generic;
using Tyc.Interface.Response;

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

    [Route("/consentimientos/consentimiento", "GET")]
    public class ObtenerFormularioConsentimiento : IReturn<ApiResponse<FormularioConsentimientoRS>>
    {
        public string Subdominio { get; set; }
        public string Id { get; set; }
    }

    public class PoliticaAceptadaItem
    {
        public int Id { get; set; }
        public string TipoTexto { get; set; }
    }


}
