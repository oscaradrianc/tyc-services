using Mapster;
using System;
using Tyc.Interface.Request;
using Tyc.Interface.Response;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Textos.Mappings;

public class TextoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Entity a Response
        config.NewConfig<Texto, TextoResponse>()
            .Map(dest => dest.Id, src => src.TextText)
            .Map(dest => dest.EmpresaId, src => src.EmpresaId)
            .Map(dest => dest.UsuarioId, src => src.UsuaUsuario)
            .Map(dest => dest.TipoTexto, src => src.TextTipoTexto)
            .Map(dest => dest.TextoTerminos, src => src.TextTextoDelosTerminos)
            .Map(dest => dest.Estado, src => src.TextEstado)
            .Map(dest => dest.FechaCreacion, src => src.TextFechaCreacion);

        // CreateRequest a Entity
        config.NewConfig<CreateTexto, Texto>()
            .Map(dest => dest.EmpresaId, src => src.EmpresaId)
            .Map(dest => dest.TextTipoTexto, src => src.TipoTexto)
            .Map(dest => dest.TextTextoDelosTerminos, src => src.TextoTerminos)
            .Map(dest => dest.TextEstado, src => EstadoTexto.Activo)
            .Map(dest => dest.TextFechaCreacion, src => DateTime.UtcNow)
            .Ignore(dest => dest.TextText)
            .Ignore(dest => dest.UsuaUsuario);

        // UpdateRequest a Entity
        config.NewConfig<UpdateTexto, Texto>()
            .Map(dest => dest.TextText, src => src.Id)
            .Map(dest => dest.TextTipoTexto, src => src.TipoTexto)
            .Map(dest => dest.TextTextoDelosTerminos, src => src.TextoTerminos)
            .Map(dest => dest.TextEstado, src => src.Estado)
            .Ignore(dest => dest.EmpresaId)
            .Ignore(dest => dest.TextFechaCreacion);
    }
}