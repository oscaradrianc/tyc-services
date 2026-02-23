using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

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
                    detalleAsignacion.Estado = "COMPLETED";
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
}