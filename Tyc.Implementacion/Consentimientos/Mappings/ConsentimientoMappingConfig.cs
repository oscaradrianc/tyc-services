using Mapster;
using Tyc.Modelo.Contexto;
using Tyc.Interface.Request;

namespace Tyc.Implementacion.Consentimientos.Mappings;
        
public class ConsentimientoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ConsentimientoRQ, Consentimiento>()
            .Map(dest => dest.NombreCliente, src => src.Nombres)
            .Map(dest => dest.ApellidoCliente, src => src.Apellidos)
            .Map(dest => dest.EmailCliente, src => src.Email)
            .Map(dest => dest.MovilCliente, src => src.Telefono)
            .Map(dest => dest.IdentificacionCliente, src => src.Identificacion)
            .Map(dest => dest.TipoIdentificacion1, src => src.TipoIdentificacion)
            .Map(dest => dest.MedioAceptacion, src => src.Medio);
    }
}


