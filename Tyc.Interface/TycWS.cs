using Administrador.ServiceLogs.Auth;
using MapsterMapper;
using ServiceStack;
using ServiceStack.Host;
using System.Collections.Generic;
using Tyc.Interface.Repositories;
using Tyc.Interface.Request;
using Tyc.Interface.Response;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;


namespace Tyc.Interface;

[Authenticate]
public class TycWS : Service
{
    private readonly IMapper _mapper;
    private readonly IConsentimientoService _consentimientoService;

    public TycWS(
        IConsentimientoService consentimientoService,
        IConsentimientoRepository repository,
        IMapper mapper)
    {
        _mapper = mapper;
        _consentimientoService = consentimientoService;
    }

    public ApiResponse<ConfirmacionConsentimientoRS> Get(GetConsentimiento request)
    {        
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var result = _consentimientoService.ObtenerConfirmacionConsentimiento(dbSigo, request.Id);

            if (result == null)
                throw HttpError.NotFound($"Consentimiento {request.Id} no encontrado");

            return new ApiResponse<ConfirmacionConsentimientoRS>
            {
                Data = result,
                Mensaje = "",
                Success = true
            };
        }
    }

    public ApiResponse<int> Post(ConsentimientoRQ request)
    {       
        // UserSession va por defecto           
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Consentimiento>(request);
            entity.UsuaUsuario = int.Parse(userSession.IDUsuario);
            entity.EmpresaId = (int)userSession.IDEmpresa; 

            var id = _consentimientoService.CrearConsentimiento(dbSigo, entity);

            return new ApiResponse<int>
            {
                Data = id,
                Mensaje = "Consentimiento creado exitosamente",
                Success = true
            };
        }

    }

    public ApiResponse<bool> Put(ActualizarConsentimientoConFirma request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var actualizado = _consentimientoService.ActualizarConsentimientoConFirma(dbSigo, request);

            if (!actualizado)
                throw HttpError.NotFound($"No se pudo actualizar el consentimiento {request.ConsentimientoId}");

            return new ApiResponse<bool>
            {
                Success = actualizado,
                Mensaje = "Consentimiento actualizado con firma exitosamente"
            };
        }
    }

    public ApiResponse<List<ConsentimientoListItemRS>> Get(ListarConsentimientosRQ request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();
        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var resultado = _consentimientoService.ListarConsentimientos(
                dbSigo,
                request.Fecha,
                request.Estado
            );

            return new ApiResponse<List<ConsentimientoListItemRS>>
            {
                Data = resultado,
                Mensaje = $"Se encontraron {resultado.Count} consentimientos",
                Success = true
            };
        }
    }
}
