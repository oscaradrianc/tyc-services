using ServiceStack;
using System;

namespace Tyc.Interface.Request;

[Route("/consentimiento/{ConsentimientoId}/pdf/{TextoId}", "GET")]
public class GetConsentimientoPdf : IReturn<byte[]>
{
    public Guid ConsentimientoId { get; set; }
    public int TextoId { get; set; }
}

[Route("/consentimientos/periodo/{Periodo}/empresa/{EmpresaId}/pdf", "GET")]
public class GetConsentimientosPorPeriodoPdf : IReturn<byte[]>
{
    public string Periodo { get; set; }
    public int EmpresaId { get; set; }
    public string Estado { get; set; } = "T";
}
