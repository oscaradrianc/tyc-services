using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Consentimientos.Repositories;

public class ConsentimientoRepository : IConsentimientoRepository
{
    public Consentimiento GetById(TycBaseContext context, int id)
    {
        return context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.Id == id);
    }

    public Consentimiento GetByGuid(TycBaseContext context, Guid guid)
    {
        return context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.GuId == guid);
    }
    public Consentimiento CrearConsentimiento(TycBaseContext context, Consentimiento consentimientoEntity)
    {     
            context.GetTable<Consentimiento>().InsertOnSubmit(consentimientoEntity);
            context.SubmitChanges();
            return consentimientoEntity;        
    }

    public bool ActualizarAceptaciones(
    TycBaseContext context,
    Guid consentimientoId,
    string medio,    
    List<string> opcionesContactabilidad,
    Dictionary<string, int> politicasAceptadas,
    DateTime fechaAceptacion, string estado)
    {
        var entity = context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.GuId == consentimientoId);

        if (entity == null)
            return false;

        // Actualizar fecha de aceptación
        entity.FechaAceptacion = fechaAceptacion;
        entity.MedioAceptacion = medio;
        entity.Estado = estado;

        // Procesar opciones de contactabilidad
        entity.ContactabilidadEmail = opcionesContactabilidad?.Contains("Email") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ContactabilidadMovil = opcionesContactabilidad?.Contains("Movil") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ContactabilidadSms = opcionesContactabilidad?.Contains("SMS") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ContactabilidadWhatsapp = opcionesContactabilidad?.Contains("WhatsApp") == true ? OpcionSiNo.Si : OpcionSiNo.No;

        // Procesar políticas aceptadas
        if (politicasAceptadas != null)
        {
            // TITULOTRATAMENTODATOS
            if (politicasAceptadas.TryGetValue("TITULOTRATAMENTODATOS", out int value))
            {
                entity.AceptoTYC = OpcionSiNo.Si;
                entity.TerminosEmpresaId = value;
            }
            else
            {
                entity.AceptoTYC = OpcionSiNo.No;
                entity.TerminosEmpresaId = null;
            }

            // TITULOCOMPARTIRDATOS
            if (politicasAceptadas.TryGetValue("TITULOCOMPARTIRDATOS", out int value1))
            {
                entity.AceptoCompartirInfo   = OpcionSiNo.Si;
                entity.CompartirInfoId = value1;
            }
            else
            {
                entity.AceptoCompartirInfo = OpcionSiNo.No;
                entity.CompartirInfoId = null;
            }

            // TITULOTERMINOSOFERTAS
            if (politicasAceptadas.TryGetValue("TITULOTERMINOSOFERTAS", out int value2))
            {
                entity.AceptoRecibirOfertas = OpcionSiNo.Si;
                entity.RecibirOfertasId = value2;
            }
            else
            {
                entity.AceptoRecibirOfertas = OpcionSiNo.No;
                entity.RecibirOfertasId = null;
            }
        }

        context.SubmitChanges();
        return true;
    }

    public bool Exists(TycBaseContext context, Guid id)
    {
        return context.GetTable<Consentimiento>()
            .Any(x => x.GuId == id);
    }

    public TipoIdentificacion GetTipoIdentificacion(TycBaseContext context, int empresaId, int tipoDocumentoId)
    {
        return context.GetTable<TipoIdentificacion>()
            .FirstOrDefault(x => x.EmpresaId == empresaId && x.TipoIdentificacionId == tipoDocumentoId);  
    }    

    public List<Consentimiento> ListarPorFiltros(TycBaseContext context, DateTime? fecha, string? estado)
    {
        var query = context.GetTable<Consentimiento>().AsQueryable();

        // Filtro por fecha de creación (fecha exacta - solo el día)
        if (fecha.HasValue)
        {
            var fechaInicio = fecha.Value.Date;
            var fechaFin = fechaInicio.AddDays(1);

            query = query.Where(x => x.FechaCreacion >= fechaInicio
                                  && x.FechaCreacion < fechaFin);
        }

        // Filtro por estado
        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (estado.Equals("F", StringComparison.OrdinalIgnoreCase))
            {
                // Firmado: tiene fecha de aceptación
                query = query.Where(x => x.FechaAceptacion != null);
            }
            else if (estado.Equals("P", StringComparison.OrdinalIgnoreCase))
            {
                // Pendiente: no tiene fecha de aceptación
                query = query.Where(x => x.FechaAceptacion == null);
            }
        }

        return query.OrderByDescending(x => x.FechaCreacion).ToList();
    }

    public List<Consentimiento> ListarPorEmpresa(TycBaseContext context, int empresaId, DateTime? fecha, string? estado)
    {
        var query = context.GetTable<Consentimiento>()
            .Where(x => x.EmpresaId == empresaId);

        if (fecha.HasValue)
        {
            var fechaInicio = fecha.Value.Date;
            var fechaFin = fechaInicio.AddDays(1);
            query = query.Where(x => x.FechaCreacion >= fechaInicio && x.FechaCreacion < fechaFin);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = estado.ToUpper() switch
            {
                "F" => query.Where(x => x.FechaAceptacion != null && x.Estado == "F"),
                "P" => query.Where(x => x.FechaAceptacion == null),
                "R" => query.Where(x => x.Estado == "R"),
                _ => query
            };
        }

        return query.OrderByDescending(x => x.FechaCreacion).ToList();
    }
}