using Mapster;
using ServiceStack;
using Tycws.Api.Features.Consentimientos.Contracts;
using Tycws.Api.Features.Consentimientos.Entities;

namespace Tycws.Api.Features.Consentimientos.Mappings;

public class ConsentimientoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ConsentimientoRQ, Consentimiento>()
            .Map(dest => dest.ConsFechaCreacionConsentimiento, src => DateTime.UtcNow)
            .Map(dest => dest.ConsGuid, src => Guid.NewGuid());
    }
}
