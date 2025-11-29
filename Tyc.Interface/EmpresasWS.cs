using Administrador.ServiceLogs.Auth;
using MapsterMapper;
using ServiceStack;
using Tyc.Interface.Request;
using Tyc.Interface.Response;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using static Tyc.Modelo.TycBaseContext;

namespace Tyc.Interface;

[Authenticate]
public class EmpresasWS : Service
{
    private readonly IMapper _mapper;
    private readonly IEmpresaService _EmpresaService;

    public EmpresasWS(
        IEmpresaService EmpresaService,
        IMapper mapper)
    {
        _EmpresaService = EmpresaService;
        _mapper = mapper;
    }

    public ApiResponse<EmpresaResponse> Get(GetEmpresa request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var result = _EmpresaService.ObtenerEmpresaPorId(dbSigo, request.Id);

            if (result == null)
                throw HttpError.NotFound($"Empresa {request.Id} no encontrada");

            return new ApiResponse<EmpresaResponse>
            {
                Success = true,
                Mensaje = "",
                Data = result
            };
        }
    }

    public ApiResponse<int> Post(CreateEmpresa request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Empresa>(request);
            var id = _EmpresaService.CrearEmpresa(dbSigo, entity);

            return new ApiResponse<int>
            {
                Data = id,
                Mensaje = "Empresa creada exitosamente",
                Success = true
            };
        }
    }

    public ApiResponse<bool> Put(UpdateEmpresa request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Empresa>(request);
            var updated = _EmpresaService.ActualizarEmpresa(dbSigo, entity);

            if (!updated)
                throw HttpError.NotFound($"Empresa {request.Id} no encontrada");

            return new ApiResponse<bool>
            {
                Data = true,
                Mensaje = "Empresa actualizada exitosamente",
                Success = true
            };
        }
    }
}