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
            .FirstOrDefault(x => x.ConsConsecuencia == id);
    }

    public Consentimiento GetByGuid(TycBaseContext context, Guid guid)
    {
        return context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.ConsGuid == guid);
    }
    public Consentimiento CrearConsentimiento(TycBaseContext context, Consentimiento consentimientoEntity)
    {     
            context.GetTable<Consentimiento>().InsertOnSubmit(consentimientoEntity);
            context.SubmitChanges();
            return consentimientoEntity;        
    }

    public bool ActualizarAceptaciones(
    TycBaseContext context,
    int consentimientoId,
    List<string> opcionesContactabilidad,
    Dictionary<string, int> politicasAceptadas,
    DateTime fechaAceptacion)
    {
        var entity = context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.ConsConsecuencia == consentimientoId);

        if (entity == null)
            return false;

        // Actualizar fecha de aceptación
        entity.ConsFechaAceptacionConsentimiento = fechaAceptacion;

        // Procesar opciones de contactabilidad
        entity.ConsContactabilidadEmail = opcionesContactabilidad?.Contains("Email") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ConsContactabilidadMovil = opcionesContactabilidad?.Contains("Movil") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ConsContactabilidadSms = opcionesContactabilidad?.Contains("SMS") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ConsContactabilidadWhatsapp = opcionesContactabilidad?.Contains("WhatsApp") == true ? OpcionSiNo.Si : OpcionSiNo.No;

        // Procesar políticas aceptadas
        if (politicasAceptadas != null)
        {
            // TITULOTYC
            if (politicasAceptadas.ContainsKey("TITULOTYC"))
            {
                entity.ConsAceptoTerminosEmpresa = OpcionSiNo.Si;
                entity.TextTerminosEmpresa = politicasAceptadas["TITULOTYC"];
            }
            else
            {
                entity.ConsAceptoTerminosEmpresa = OpcionSiNo.No;
                entity.TextTerminosEmpresa = null;
            }

            // TITULOCOMPARTIRDATOS
            if (politicasAceptadas.ContainsKey("TITULOCOMPARTIRDATOS"))
            {
                entity.ConsAceptoTerminosCompartirInfo = OpcionSiNo.Si;
                entity.TgeTextTerminoCompartirInfo = politicasAceptadas["TITULOCOMPARTIRDATOS"];
            }
            else
            {
                entity.ConsAceptoTerminosCompartirInfo = OpcionSiNo.No;
                entity.TgeTextTerminoCompartirInfo = null;
            }

            // TITULOTERMINOSOFERTAS
            if (politicasAceptadas.ContainsKey("TITULOTERMINOSOFERTAS"))
            {
                entity.ConsAceptoTerminosRecibirOfertas = OpcionSiNo.Si;
                entity.TgeTextOfertas = politicasAceptadas["TITULOTERMINOSOFERTAS"];
            }
            else
            {
                entity.ConsAceptoTerminosRecibirOfertas = OpcionSiNo.No;
                entity.TgeTextOfertas = null;
            }
        }

        context.SubmitChanges();
        return true;
    }

    public bool Exists(TycBaseContext context, int id)
    {
        return context.GetTable<Consentimiento>()
            .Any(x => x.ConsConsecuencia == id);
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

            query = query.Where(x => x.ConsFechaCreacionConsentimiento >= fechaInicio
                                  && x.ConsFechaCreacionConsentimiento < fechaFin);
        }

        // Filtro por estado
        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (estado.Equals("F", StringComparison.OrdinalIgnoreCase))
            {
                // Firmado: tiene fecha de aceptación
                query = query.Where(x => x.ConsFechaAceptacionConsentimiento != null);
            }
            else if (estado.Equals("P", StringComparison.OrdinalIgnoreCase))
            {
                // Pendiente: no tiene fecha de aceptación
                query = query.Where(x => x.ConsFechaAceptacionConsentimiento == null);
            }
        }

        return query.OrderByDescending(x => x.ConsFechaCreacionConsentimiento).ToList();
    }
}