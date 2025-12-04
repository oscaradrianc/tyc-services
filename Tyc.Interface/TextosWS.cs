using Administrador.ServiceLogs.Auth;
using MapsterMapper;
using ServiceStack;
using ServiceStack.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using Tyc.Interface.Request;
using Tyc.Interface.Response;
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

    public TextoResponse Get(GetTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var result = _textoService.ObtenerTextoPorId(dbSigo, request.Id);

            if (result == null)
                throw HttpError.NotFound($"Texto {request.Id} no encontrado");

            return result;
        }
    }

    public ApiResponse<List<TextoResponse>> Get(ListTextos request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            if (request.EmpresaId.HasValue)
            {
                var result = _textoService.ObtenerTextosPorEmpresa(
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

    public TextoResponse Get(GetTextoByEmpresaYTipo request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var result = _textoService.ObtenerTextoPorEmpresaYTipo(
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
    public ApiResponse<List<TextoResponse>> Get(GetTextosByEmpresaYTipos request)
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

            var result = _textoService.ObtenerTextosPorEmpresaYTipos(
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
    public ApiResponse<List<TextoResponse>> Post(GetTextosByEmpresaYTiposPost request)
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

            var result = _textoService.ObtenerTextosPorEmpresaYTipos(
                dbSigo, request.EmpresaId, request.Tipos, request.SoloActivos);

            return new ApiResponse<List<TextoResponse>>
            {
                Data = result,
                Mensaje = $"Se encontraron {result.Count} textos",
                Success = true
            };
        }
    }

    public ApiResponse<int> Post(CreateTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Texto>(request);
            var id = _textoService.CrearTexto(dbSigo, entity, int.Parse(userSession.IDUsuario));

            return new ApiResponse<int>
            {
                Data = id,
                Mensaje = "Texto creado exitosamente",
                Success = true
            };
        }
    }

    public ApiResponse<bool> Put(UpdateTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Texto>(request);
            var updated = _textoService.ActualizarTexto(
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

    public ApiResponse<object> Delete(DeleteTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var deleted = _textoService.EliminarTexto(dbSigo, request.Id);

            if (!deleted)
                throw HttpError.NotFound($"Texto {request.Id} no encontrado");

            return new ApiResponse<object>
            {
                Success = true,
                Mensaje = "Texto eliminado exitosamente"
            };
        }
    }

    public ApiResponse<object> Put(CambiarEstadoTexto request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var cambiado = _textoService.CambiarEstado(dbSigo, request.Id, request.Estado);

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
}