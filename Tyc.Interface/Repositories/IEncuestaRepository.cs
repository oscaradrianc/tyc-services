using System.Collections.Generic;
using Tyc.Interface.Request;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Repositories;

public interface IEncuestaRepository
{
    int CrearAsignacionConDetalles(TycBaseContext context, AsignacionEncuesta cabecera, List<DetalleAsignacion> detalles);
    int GuardarRespuestasCliente(TycBaseContext context, RespuestasEncuesta cabeceraRespuesta, List<RespuestaDetalle> detalleRespuestas);
    List<DetalleAsignacion> ObtenerNotificacionesPendientes(TycBaseContext context, int maxIntentos);
    void ActualizarEstadoNotificacion(TycBaseContext context, int idDetalle, string notificado, short intentos, string error);
    EncuestaEstructuraRS ObtenerEstructuraEncuesta(TycBaseContext context, int encuestaId);
}
