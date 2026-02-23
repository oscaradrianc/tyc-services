using System.Collections.Generic;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Repositories;

public interface IEncuestaRepository
{
    int CrearAsignacionConDetalles(TycBaseContext context, AsignacionEncuesta cabecera, List<DetalleAsignacion> detalles);
    int GuardarRespuestasCliente(TycBaseContext context, RespuestasEncuesta cabeceraRespuesta, List<RespuestaDetalle> detalleRespuestas);
}
