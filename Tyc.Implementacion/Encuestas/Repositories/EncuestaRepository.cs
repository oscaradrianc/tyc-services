using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tyc.Interface.Repositories;
using Tyc.Interface.Request;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using Tyc.Modelo.Tipos;

namespace Tyc.Implementacion.Encuestas.Repositories;

public class EncuestaRepository : IEncuestaRepository
{
    public int CrearAsignacionConDetalles(TycBaseContext context, AsignacionEncuesta cabecera, List<DetalleAsignacion> detalles)
    {
        if (context.Connection.State != ConnectionState.Open)
            context.Connection.Open();

        using (var transaction = context.Connection.BeginTransaction())
        {
            context.Transaction = transaction;

            try
            {
                // 1. Insertar Cabecera y obtener ID
                context.GetTable<AsignacionEncuesta>().InsertOnSubmit(cabecera);
                context.SubmitChanges();

                // 2. Insertar Detalles con el ID de la cabecera
                foreach (var det in detalles)
                {
                    det.AsignacionId = cabecera.IdAsignacion;
                    context.GetTable<DetalleAsignacion>().InsertOnSubmit(det);
                }
                context.SubmitChanges();

                transaction.Commit();
                return cabecera.IdAsignacion;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public int GuardarRespuestasCliente(TycBaseContext context, RespuestasEncuesta cabeceraRespuesta, List<RespuestaDetalle> detalleRespuestas)
    {
        if (context.Connection.State != ConnectionState.Open)
            context.Connection.Open();

        using (var transaction = context.Connection.BeginTransaction())
        {
            context.Transaction = transaction;

            try
            {
                // 1. Insertar Cabecera de Respuesta
                context.GetTable<RespuestasEncuesta>().InsertOnSubmit(cabeceraRespuesta);
                context.SubmitChanges();

                // 2. Insertar cada respuesta individual
                foreach (var respuesta in detalleRespuestas)
                {
                    respuesta.RespuestaId = cabeceraRespuesta.IdRespuesta;
                    context.GetTable<RespuestaDetalle>().InsertOnSubmit(respuesta);
                }

                // 3. Actualizar el estado de la asignación a COMPLETADO
                var detalleAsignacion = context.GetTable<DetalleAsignacion>()
                                        .SingleOrDefault(x => x.IdDetalle == cabeceraRespuesta.DetalleId);
                if (detalleAsignacion != null)
                {
                    detalleAsignacion.Estado = "COMPLETADO";
                }

                context.SubmitChanges();

                transaction.Commit();
                return cabeceraRespuesta.IdRespuesta;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public List<DetalleAsignacion> ObtenerNotificacionesPendientes(TycBaseContext context, int maxIntentos)
    {
        // Traemos los que no han sido notificados y no han superado el límite de intentos
        return context.GetTable<DetalleAsignacion>()
            .Where(d => d.Notificado == "N" && d.NroIntentos < maxIntentos)
            .ToList();
    }

    public void ActualizarEstadoNotificacion(TycBaseContext context, int idDetalle, string notificado, short intentos, string error)
    {
        var detalle = context.GetTable<DetalleAsignacion>().FirstOrDefault(d => d.IdDetalle == idDetalle);
        if (detalle != null)
        {
            detalle.Notificado = notificado;
            detalle.NroIntentos = intentos;
            // Cortamos el error si es muy largo para evitar que reviente la base de datos
            detalle.Errores = error?.Length > 500 ? error.Substring(0, 500) : error;
            context.SubmitChanges();
        }
    }

    public EncuestaEstructuraRS ObtenerEstructuraEncuesta(TycBaseContext context, int encuestaId)
    {
        // 1. Obtener la encuesta cabecera
        var encuesta = context.GetTable<Encuesta>()
            .FirstOrDefault(e => e.IdEncuesta == encuestaId && e.Estado == "A");

        if (encuesta == null)
            return null;

        // 2. Obtener todas las preguntas de esa encuesta ordenadas
        var preguntas = context.GetTable<Pregunta>()
            .Where(p => p.EncuestaId == encuestaId)
            .OrderBy(p => p.Orden)
            .ToList();

        var preguntaIds = preguntas.Select(p => p.IdPregunta).ToList();

        // 3. Obtener todas las opciones que pertenezcan a las preguntas anteriores
        var opciones = context.GetTable<OpcionPregunta>()
            .Where(o => preguntaIds.Contains(o.PreguntaId))
            .ToList();

        // 4. Mapear todo hacia la estructura anidada DTO
        var resultado = new EncuestaEstructuraRS
        {
            IdEncuesta = encuesta.IdEncuesta,
            Nombre = encuesta.Nombre,
            TipoEncuesta = encuesta.TipoEncuesta,
            Descripcion = encuesta.Descripcion,
            Preguntas = preguntas.Select(p => new PreguntaEstructuraRS
            {
                IdPregunta = p.IdPregunta,
                Pregunta = p.TextoPregunta,
                TipoPregunta = p.TipoPregunta,
                Orden = p.Orden,
                EsObligatoria = p.EsObligatoria,
                ReglasValidacion = p.ReglasValidacion,
                // Filtramos las opciones que le pertenecen solo a esta pregunta
                Opciones = opciones.Where(o => o.PreguntaId == p.IdPregunta).Select(o => new OpcionEstructuraRS
                {
                    IdOpcion = o.IdOpcion,
                    Etiqueta = o.Etiqueta,
                    ExtraTexto = o.ExtraTexto
                }).ToList()
            }).ToList()
        };

        return resultado;
    }

    public DatosEncuesta ObtenerEncabezaEncuestaPorIdAsigancion(TycBaseContext context, int idDetalleAsignacion)
    {
        var encuesta = (from e in context.GetTable<Encuesta>()
                        join a in context.GetTable<AsignacionEncuesta>() on e.IdEncuesta equals a.EncuestaId
                        join d in context.GetTable<DetalleAsignacion>() on a.IdAsignacion equals d.AsignacionId
                        where d.IdDetalle == idDetalleAsignacion                      
                        select new DatosEncuesta
                        {
                            IdEncuesta = e.IdEncuesta,
                            Nombre = e.Nombre,
                            TipoEncuesta = e.TipoEncuesta,
                            Descripcion = e.Descripcion
                        }).FirstOrDefault();
        return encuesta;
    }

    public List<EmpresasBloquear> ObtenerEmpresaBloquear(TycBaseContext context)
    {
        var empresas = new List<EmpresasBloquear>();

        // 1. Definimos la consulta SQL tal cual la probaste en la BD
        string query = @"
        SELECT 
            d.empr_empr as IdEmpresa, 
            a.fecha_limite_respuesta as FechaLimiteEncuesta, 
            (current_date > a.fecha_limite_respuesta::DATE) AS Bloquear
        FROM tenc_asignacionencuesta a
        JOIN tenc_detalleasignacion d ON a.id_asignacion = d.asignacion_id
        WHERE d.estado = 'PENDIENTE' AND (current_date > a.fecha_limite_respuesta::DATE)";

        // 2. Usamos la conexión existente del contexto
        using (var command = context.Connection.CreateCommand())
        {
            command.CommandText = query;

            // Aseguramos que la conexión esté abierta
            if (context.Connection.State != ConnectionState.Open)
                context.Connection.Open();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    empresas.Add(new EmpresasBloquear
                    {
                        // Convert.ToInt32 por si el tipo en BD es numeric o bigint
                        IdEmpresa = Convert.ToInt32(reader["IdEmpresa"]),
                        FechaLimiteEncuesta = Convert.ToDateTime(reader["FechaLimiteEncuesta"]),
                        Bloquear = Convert.ToBoolean(reader["Bloquear"])
                    });
                }
            }
        }

        // Aplicamos el Distinct en memoria si la consulta SQL puede traer duplicados
        return empresas.GroupBy(e => new { e.IdEmpresa, e.FechaLimiteEncuesta, e.Bloquear })
                       .Select(g => g.First())
                       .ToList();
    }
}