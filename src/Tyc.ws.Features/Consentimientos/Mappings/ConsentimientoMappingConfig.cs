using Mapster;
using Tyc.ws.Features.Consentimientos.Contracts;
using Tyc.ws.Features.Consentimientos.Entities;

namespace Tyc.ws.Features.Consentimientos.Mappings;

public class ConsentimientoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ConsentimientoRQ, Consentimiento>()
            .Map(dest => dest.ConsNombre, src => src.Nombres)
            .Map(dest => dest.ConsApellido, src => src.Apellidos)
            .Map(dest => dest.ConsEmail, src => src.Email)
            .Map(dest => dest.ConsMovil, src => src.Telefono)
            .Map(dest => dest.ConsIdentificacion, src => src.Identificacion);
    }
}
