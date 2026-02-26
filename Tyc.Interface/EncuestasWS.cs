using Administrador.ServiceLogs.Auth;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tyc.Interface.Request;
using Tyc.Interface.Response.General;
using Tyc.Interface.Services;
using Tyc.Modelo;

namespace Tyc.Interface;

[Authenticate]
public class EncuestasWS : Service
{
    private readonly IEncuestaService _encuestaService;

    public EncuestasWS(IEncuestaService encuestaService)
    {
        _encuestaService = encuestaService;
    }

    // Endpoint para CREAR LA ASIGNACIÓN (La Campaña)
    public async Task<ApiResponse<int>> Post(CreateAsignacionRQ request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            if (request.EmpresasIds == null || request.EmpresasIds.Count == 0)
                throw HttpError.BadRequest("Debe seleccionar al menos una agencia (empresa) para asignar la encuesta.");

            int usuarioId = int.Parse(userSession.IDUsuario);

            var idGenerado = await _encuestaService.CrearAsignacion(
                dbSigo,
                request.EncuestaId,
                request.Nombre,
                request.FechaLimiteRespuesta,
                request.Observaciones,
                request.EmpresasIds,
                usuarioId
            );

            return new ApiResponse<int>
            {
                Success = true,
                Mensaje = "Asignación de encuesta creada exitosamente",
                Data = idGenerado
            };
        }
    }

    // Endpoint para GUARDAR LAS RESPUESTAS (Del cliente)
    public async Task<ApiResponse<int>> PostAsync(SaveRespuestaRQ request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            if (request.Respuestas == null || request.Respuestas.Count == 0)
                throw HttpError.BadRequest("No se enviaron respuestas válidas.");

            int usuarioId = int.Parse(userSession.IDUsuario);
            int empresaId = (int)userSession.IDEmpresa;

            var idRespuestaGenerada = await _encuestaService.GuardarRespuesta(
                dbSigo,
                empresaId,
                request.DetalleId,
                request.Respuestas,
                usuarioId
            );

            return new ApiResponse<int>
            {
                Success = true,
                Mensaje = "Respuestas guardadas exitosamente",
                Data = idRespuestaGenerada
            };
        }
    }

    public ApiResponse<EncuestaEstructuraRS> Get(GetEncuestaEstructuraRQ request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var estructura = _encuestaService.ObtenerEstructuraEncuesta(dbSigo, request.EncuestaId);

            if (estructura == null)
                throw HttpError.NotFound($"La encuesta con ID {request.EncuestaId} no fue encontrada o está inactiva.");

            return new ApiResponse<EncuestaEstructuraRS>
            {
                Success = true,
                Mensaje = "Estructura obtenida exitosamente",
                Data = estructura
            };
        }
    }
}