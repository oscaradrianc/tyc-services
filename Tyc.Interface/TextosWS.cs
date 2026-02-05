using Administrador.ServiceLogs.Auth;
using MapsterMapper;
using ServiceStack;
using ServiceStack.Host;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Tyc.Interface.Request;
using Tyc.Interface.Response.General;
using Tyc.Interface.Response.Textos;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using static Tyc.Modelo.TycBaseContext;

namespace Tyc.Interface;

[Authenticate]
public class TextosWS : Service
{
    private readonly IMapper _mapper;
    private readonly ITextoService _textoService;

    public TextosWS(
        ITextoService textoService,
        IMapper mapper)
    {
        _textoService = textoService;
        _mapper = mapper;
    }

    public async Task<TextoResponse> Get(GetTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var result = await _textoService.ObtenerTextoPorIdAsync(dbSigo, request.Id);

            if (result == null)
                throw HttpError.NotFound($"Texto {request.Id} no encontrado");

            return result;
        }
    }

    public async Task<ApiResponse<List<TextoResponse>>> Get(ListTextos request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            if (request.EmpresaId.HasValue)
            {
                var result = await _textoService.ObtenerTextosPorEmpresaAsync(
                    dbSigo, request.EmpresaId.Value, request.SoloActivos);

                return new ApiResponse<List<TextoResponse>>
                {
                    Data = result,
                    Mensaje = "Textos obtenidos exitosamente",
                    Success = true
                };
            }

            return new ApiResponse<List<TextoResponse>>
            {
                Data = new List<TextoResponse>(),
                Mensaje = "No se proporcionó EmpresaId",
                Success = false
            };
        }
    }

    public async Task<TextoResponse> Get(GetTextoByEmpresaYTipo request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var result = await _textoService.ObtenerTextoPorEmpresaYTipoAsync(
                dbSigo, request.EmpresaId, request.TipoTexto);

            if (result == null)
                throw HttpError.NotFound(
                    $"No se encontró texto de tipo '{request.TipoTexto}' para la Empresa {request.EmpresaId}");

            return result;
        }
    }

    /// <summary>
    /// GET /textos/Empresa/{EmpresaId}/tipos?tipos=CORREO_SALUDO,CORREO_TEXTOALTERNO
    /// </summary>
    public async Task<ApiResponse<List<TextoResponse>>> Get(GetTextosByEmpresaYTipos request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            if (string.IsNullOrWhiteSpace(request.Tipos))
            {
                return new ApiResponse<List<TextoResponse>>
                {
                    Data = new List<TextoResponse>(),
                    Mensaje = "No se proporcionaron tipos de texto",
                    Success = false
                };
            }

            // Parsear tipos separados por coma
            var tiposList = request.Tipos
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            var result = await _textoService.ObtenerTextosPorEmpresaYTiposAsync(
                dbSigo, request.EmpresaId, tiposList, request.SoloActivos);

            return new ApiResponse<List<TextoResponse>>
            {
                Data = result,
                Mensaje = $"Se encontraron {result.Count} textos",
                Success = true
            };
        }
    }

    /// <summary>
    /// POST /textos/Empresa/{EmpresaId}/tipos
    /// Body: { "Tipos": ["CORREO_SALUDO", "CORREO_TEXTOALTERNO"], "SoloActivos": true }
    /// </summary>
    public async Task<ApiResponse<List<TextoResponse>>> Post(GetTextosByEmpresaYTiposPost request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            if (request.Tipos == null || !request.Tipos.Any())
            {
                return new ApiResponse<List<TextoResponse>>
                {
                    Data = new List<TextoResponse>(),
                    Mensaje = "No se proporcionaron tipos de texto",
                    Success = false
                };
            }

            var result = await _textoService.ObtenerTextosPorEmpresaYTiposAsync(
                dbSigo, request.EmpresaId, request.Tipos, request.SoloActivos);

            return new ApiResponse<List<TextoResponse>>
            {
                Data = result,
                Mensaje = $"Se encontraron {result.Count} textos",
                Success = true
            };
        }
    }

    public async Task<ApiResponse<int>> Post(CreateTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Texto>(request);
            var id = await _textoService.CrearTextoAsync(dbSigo, entity, int.Parse(userSession.IDUsuario));

            return new ApiResponse<int>
            {
                Data = id,
                Mensaje = "Texto creado exitosamente",
                Success = true
            };
        }
    }

    public async Task<ApiResponse<bool>> Put(UpdateTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Texto>(request);
            var updated = await _textoService.ActualizarTextoAsync(
                dbSigo, entity, int.Parse(userSession.IDUsuario));

            if (!updated)
                throw HttpError.NotFound($"Texto {request.Id} no encontrado");

            return new ApiResponse<bool>
            {
                Data = true,
                Mensaje = "Texto actualizado exitosamente",
                Success = true
            };
        }
    }

    public async Task<ApiResponse<object>> Delete(DeleteTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var deleted = await _textoService.EliminarTextoAsync(dbSigo, request.Id);

            if (!deleted)
                throw HttpError.NotFound($"Texto {request.Id} no encontrado");

            return new ApiResponse<object>
            {
                Success = true,
                Mensaje = "Texto eliminado exitosamente"
            };
        }
    }

    public async Task<ApiResponse<object>> Put(CambiarEstadoTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var cambiado = await _textoService.CambiarEstadoAsync(dbSigo, request.Id, request.Estado);

            if (!cambiado)
                throw HttpError.NotFound($"Texto {request.Id} no encontrado");

            return new ApiResponse<object>
            {
                Success = true,
                Mensaje = "Estado actualizado exitosamente",
                Data = request.Id
            };
        }
    }

    public async Task<ApiResponse<GuardarListaTextosRS>> Put(GuardarListaTextos request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            if (request.Items == null || !request.Items.Any())
            {
                return new ApiResponse<GuardarListaTextosRS>
                {
                    Success = false,
                    Mensaje = "No se proporcionaron items para guardar"
                };
            }

            var result = await _textoService.GuardarListaAsync(
                dbSigo,
                request.Items,
                int.Parse(userSession.IDUsuario),
                Convert.ToInt32(userSession.IDEmpresa));

            return new ApiResponse<GuardarListaTextosRS>
            {
                Data = result,
                Success = true,
                Mensaje = $"Procesados: {result.Insertados} insertados, {result.Actualizados} actualizados"
            };
        }
    }
}