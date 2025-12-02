using ServiceStack;
using System;
using System.Collections.Generic;
using Tyc.Interface.Response;
using Tyc.Modelo.Tipos;

namespace Tyc.Interface.Request
{
    public class ConsentimientoPublicoRQ
    {
        [Route("/consentimientos/{ConsentimientoId}/", "PUT")]
        public class ActualizarConsentimiento : IReturn<ApiResponse<bool>>
        {
            public string Subdominio { get; set; }
            public string Id { get; set; }
            public int ConsentimientoId { get; set; }
            public List<PoliticaAceptadaItem> PoliticasAceptadas { get; set; }
            public List<string> OpcionesContactabilidad { get; set; }
            public DateTime FechaFirma { get; set; }
            public string Dispositivo { get; set; }
        }
    }

    [Route("/consentimientos/consentimiento", "GET")]
    public class ObtenerFormularioConsentimiento : IReturn<ApiResponse<FormularioConsentimientoRS>>
    {
        public string Subdominio { get; set; }
        public string Id { get; set; }
    }
}