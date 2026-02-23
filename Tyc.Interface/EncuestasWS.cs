using Administrador.ServiceLogs.Auth;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;
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
    public ApiResponse<int> Post(CreateAsignacionRQ request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            if (request.EmpresasIds == null || request.EmpresasIds.Count == 0)
                throw HttpError.BadRequest("Debe seleccionar al menos una agencia (empresa) para asignar la encuesta.");

            int usuarioId = int.Parse(userSession.IDUsuario);

            var idGenerado = _encuestaService.CrearAsignacion(
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
    public ApiResponse<int> Post(SaveRespuestaRQ request)
    {
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            if (request.Respuestas == null || request.Respuestas.Count == 0)
                throw HttpError.BadRequest("No se enviaron respuestas válidas.");

            int usuarioId = int.Parse(userSession.IDUsuario);

            var idRespuestaGenerada = _encuestaService.GuardarRespuesta(
                dbSigo,
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
}