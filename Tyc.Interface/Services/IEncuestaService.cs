using System;
using System.Collections.Generic;
using System.Text;
using Tyc.Modelo;

namespace Tyc.Interface.Services;

public interface IEncuestaService
{
    int CrearAsignacion(TycBaseContext context, int encuestaId, string nombre, DateTime fechaLimite, string observaciones, List<int> empresasIds, int usuarioId);
    int GuardarRespuesta(TycBaseContext context, int detalleId, List<Request.RespuestaItemRQ> respuestas, int usuarioId);
}
