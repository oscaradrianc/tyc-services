using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyc.Interface.Request;

[Route("/consentimiento/{ConsentimientoId}/pdf/{TextoId}", "GET")]
public class GetConsentimientoPdf : IReturn<byte[]>
{
    public Guid ConsentimientoId { get; set; }
    public int TextoId { get; set; }
}
