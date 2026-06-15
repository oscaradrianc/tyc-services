using Administrador.ServiceLogs.Auth;
using MapsterMapper;
using ServiceStack;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tyc.Interface.Repositories;
using Tyc.Interface.Request;
using Tyc.Interface.Response.Consentimientos;
using Tyc.Interface.Response.General;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;


namespace Tyc.Interface;

[Authenticate]
public class TycWS : Service
{
    private readonly IMapper _mapper;
    private readonly IConsentimientoService _consentimientoService;
    private readonly IConsentimientoRepository _repository;
    private readonly IPdfService _pdfService;
    private readonly IUsuarioService _usuarioService;

    public TycWS(
        IConsentimientoService consentimientoService,
        IConsentimientoRepository repository,
        IMapper mapper,
        IPdfService pdfService,
        IUsuarioService usuarioService)
    {
        _mapper = mapper;
        _consentimientoService = consentimientoService;
        _repository = repository;
        _pdfService = pdfService;
        _usuarioService = usuarioService;
    }

    private bool EsSuperUsuario(TycBaseContext dbSigo, CustomUserSession userSession)
    {
        var permisos = _usuarioService.GetPermisosUsuario(
            dbSigo, Convert.ToInt32(userSession.IDEmpresa), int.Parse(userSession.IDUsuario));

        return permisos.Success && permisos.Data?.UsuaEsSuperUsuario == "SI";
    }

    /// <summary>
    /// Verifica que el consentimiento exista y pertenezca a la empresa de la sesión
    /// (salvo super-usuarios). Responde NotFound en ambos casos para no revelar
    /// la existencia de recursos de otras empresas.
    /// </summary>
    private async Task ValidarPertenenciaAsync(TycBaseContext dbSigo, CustomUserSession userSession, Guid consentimientoGuid)
    {
        var entity = await _repository.GetByGuidAsync(dbSigo, consentimientoGuid);

        if (entity == null ||
            (entity.EmpresaId != Convert.ToInt32(userSession.IDEmpresa) && !EsSuperUsuario(dbSigo, userSession)))
        {
            throw HttpError.NotFound($"Consentimiento {consentimientoGuid} no encontrado");
        }
    }

    public async Task<ApiResponse<ConfirmacionConsentimientoRS>> Get(GetConsentimiento request)
    {        
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            await ValidarPertenenciaAsync(dbSigo, userSession, request.Id);

            var result = await _consentimientoService.ObtenerConfirmacionConsentimientoAsync(dbSigo, request.Id);

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

    public async Task<object> Post(ConsentimientoRQ request)
    {
        // UserSession va por defecto
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Consentimiento>(request);
            entity.UsuarioId = int.Parse(userSession.IDUsuario);
            entity.EmpresaId = (int)userSession.IDEmpresa;

            var (id, existente) = await _consentimientoService.CrearConsentimientoAsync(
                dbSigo,
                entity,
                request.ForceInsert);

            if (existente != null)
            {
                return new HttpResult(existente, "application/json")
                {
                    StatusCode = System.Net.HttpStatusCode.Conflict
                };
            }

            return new ApiResponse<Guid>
            {
                Data = id.Value,
                Mensaje = "Consentimiento creado exitosamente",
                Success = true
            };
        }
    }

    public async Task<ApiResponse<bool>> Put(ActualizarConsentimientoConFirma request)
    {
        string clientIp = string.Empty;

        if (IPAddress.TryParse(Request.UserHostAddress, out var ip))
        {
            clientIp = ip.IsIPv4MappedToIPv6
                ? ip.MapToIPv4().ToString()
                : ip.ToString();
        }

        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            request.IpClienteFirma = clientIp;
            var res = await _consentimientoService.ActualizarConsentimientoConFirmaAsync(dbSigo, request);

            if (!res.Status)
            {
                throw HttpError.Validation("ValidacionConsentimiento", res.Message, null);
            }

            return new ApiResponse<bool>
            {
                Success = res.Status,
                Mensaje = "Consentimiento actualizado con firma exitosamente"
            };
        }
    }

    public async Task<ApiResponse<List<ConsentimientoListItemRS>>> Get(ListarConsentimientosRQ request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();
        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var resultado = await _consentimientoService.ListarConsentimientosAsync(
                dbSigo,
                request.Fecha,
                request.Estado,
                Convert.ToInt32(userSession.IDEmpresa), int.Parse(userSession.IDUsuario)
            );

            return new ApiResponse<List<ConsentimientoListItemRS>>
            {
                Data = resultado,
                Mensaje = $"Se encontraron {resultado.Count} consentimientos",
                Success = true
            };
        }
    }

    public async Task<ApiResponse<List<ConsentimientosRS>>> Get(ListarConsentimientosPorEmpresaRQ request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            // El EmpresaId del request solo es válido si coincide con la empresa de
            // la sesión; cruzar empresas (incluido -1 = todas) exige super-usuario.
            int empresaSesion = Convert.ToInt32(userSession.IDEmpresa);
            if (request.EmpresaId != empresaSesion && !EsSuperUsuario(dbSigo, userSession))
            {
                throw HttpError.Forbidden("No tiene permisos para consultar consentimientos de otra empresa.");
            }

            var resultado = await _consentimientoService.ListarConsentimientosPorEmpresaAsync(
                dbSigo,
                request.EmpresaId,
                request.FechaInicial,
                request.FechaFinal,
                request.Estado,
                int.Parse(userSession.IDUsuario),
                request.TerminoBusqueda
            );

            return new ApiResponse<List<ConsentimientosRS>>
            {
                Data = resultado,
                Mensaje = $"Se encontraron {resultado.Count} consentimientos",
                Success = true
            };
        }
    }

    // Agregar en TycWS.cs
    public async Task<IHttpResult> Get(GetConsentimientoPdf request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            await ValidarPertenenciaAsync(dbSigo, userSession, request.ConsentimientoId);

            var pdfBytes = await _pdfService.GenerarConsentimientoPdfAsync(
                dbSigo,
                request.ConsentimientoId,
                request.TextoId);

            return new HttpResult(pdfBytes, "application/pdf")
            {
                Headers =
            {
            }
            };
        }
    }

    public async Task<ApiResponse<bool>> Delete(DeleteConsentimientoRQ request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var permisos = _usuarioService.GetPermisosUsuario(dbSigo, Convert.ToInt32(userSession.IDEmpresa), int.Parse(userSession.IDUsuario));
            
            if (!permisos.Success || permisos.Data.UsuaPuedeCrearUsuariosAdmin != "SI")
            {
                throw HttpError.Unauthorized("El usuario no tiene permisos para eliminar registros.");
            }

            // Verifica existencia y que pertenezca a la empresa de la sesión
            await ValidarPertenenciaAsync(dbSigo, userSession, request.Id);

            var res = await _consentimientoService.EliminarConsentimientoAsync(dbSigo, request.Id);

            return new ApiResponse<bool>
            {
                Data = res,
                Mensaje = res ? "Consentimiento eliminado exitosamente" : "No fue posible eliminar el registro",
                Success = res
            };
        }
    }

    public async Task<IHttpResult> Get(GetConsentimientosPorPeriodoPdf request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            int empresaSesion = Convert.ToInt32(userSession.IDEmpresa);
            if (request.EmpresaId != empresaSesion && !EsSuperUsuario(dbSigo, userSession))
            {
                throw HttpError.Forbidden("No tiene permisos para consultar consentimientos de otra empresa.");
            }

            var pdfBytes = await _pdfService.GenerarConsentimientosPorPeriodoPdfAsync(
                dbSigo,
                request.Periodo,
                request.EmpresaId,
                request.Estado);

            var nombreArchivo = $"Consentimientos_{request.EmpresaId}_{request.Periodo}.pdf";

            return new HttpResult(pdfBytes, "application/pdf")
            {
                Headers =
                {
                    ["Content-Disposition"] = $"attachment; filename=\"{nombreArchivo}\""
                }
            };
        }
    }
}
