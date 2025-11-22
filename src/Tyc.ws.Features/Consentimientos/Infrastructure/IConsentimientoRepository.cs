using Tyc.ws.Features.Consentimientos.Entities;

namespace Tyc.ws.Features.Consentimientos.Infrastructure;
internal interface IConsentimientoRepository
{
    Task<Consentimiento> CrearConsentimientoAsync(Consentimiento datosConsentimiento,
       CancellationToken cancellationToken);
}
