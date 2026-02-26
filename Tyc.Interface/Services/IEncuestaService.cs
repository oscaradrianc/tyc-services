using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tyc.Interface.Request;
using Tyc.Modelo;

namespace Tyc.Interface.Services;

public interface IEncuestaService
{
    Task<int> CrearAsignacion(TycBaseContext context, int encuestaId, string nombre, DateTime fechaLimite, string observaciones, 
        List<Guid> empresasIds, int usuarioId);
    Task<int> GuardarRespuesta(TycBaseContext context, int empresaId, int detalleId, List<Request.RespuestaItemRQ> respuestas, int usuarioId);
    Task ProcesarNotificacionesPendientesAsync(TycBaseContext context);
    EncuestaEstructuraRS ObtenerEstructuraEncuesta(TycBaseContext context, int encuestaId);
}
