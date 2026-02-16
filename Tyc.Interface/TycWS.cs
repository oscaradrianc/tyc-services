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
    private readonly IPdfService _pdfService;

    public TycWS(
        IConsentimientoService consentimientoService,
        IConsentimientoRepository repository,
        IMapper mapper,
        IPdfService pdfService)
    {
        _mapper = mapper;
        _consentimientoService = consentimientoService;
        _pdfService = pdfService;
    }

    public async Task<ApiResponse<ConfirmacionConsentimientoRS>> Get(GetConsentimiento request)
    {        
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
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

    public async Task<ApiResponse<Guid>> Post(ConsentimientoRQ request)
    {       
        // UserSession va por defecto           
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Consentimiento>(request);
            entity.UsuarioId = int.Parse(userSession.IDUsuario);
            entity.EmpresaId = (int)userSession.IDEmpresa; 

            var id = await _consentimientoService.CrearConsentimientoAsync(dbSigo, entity);

            return new ApiResponse<Guid>
            {
                Data = id,
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
                Convert.ToInt32(userSession.IDEmpresa)
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
            var resultado = await _consentimientoService.ListarConsentimientosPorEmpresaAsync(
                dbSigo,
                request.EmpresaId,
                request.FechaInicial,
                request.FechaFinal,
                request.Estado
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
            var pdfBytes = await _pdfService.GenerarConsentimientoPdfAsync(
                dbSigo,
                request.ConsentimientoId,
                request.TextoId);

            return new HttpResult(pdfBytes, "application/pdf")
            {
                Headers =
            {
                // inline = ver en navegador, attachment = forzar descarga
                ["Content-Disposition"] = $"inline; filename=consentimiento_{request.ConsentimientoId}.pdf"
            }
            };
        }
    }
}
