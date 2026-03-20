using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tyc.Interface.Request;
using Tyc.Interface.Response.Consentimientos;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using Tyc.Modelo.Tipos;
using static Tyc.Interface.Request.ConsentimientoPublicoRQ;

namespace Tyc.Interface.Services;

public interface IConsentimientoService
{
    // Synchronous methods removed in favor of Async versions

    // Async Methods
    Task<ConfirmacionConsentimientoRS> ObtenerConfirmacionConsentimientoAsync(TycBaseContext context, Guid id);
    Task<(Guid? Id, ConsentimientoExistenteRS Existente)> CrearConsentimientoAsync(TycBaseContext context, Consentimiento entity, bool forceInsert = false);
    Task<StatusResult> ActualizarConsentimientoConFirmaAsync(TycBaseContext context, ActualizarConsentimientoConFirma request);
    Task<FormularioConsentimientoRS> ObtenerFormularioConsentimientoAsync(TycBaseContext context, string subdominio, string idEncriptado);
    Task<StatusResult> ActualizarConsentimientoAsync(TycBaseContext context, ActualizarConsentimiento request);
    Task<List<ConsentimientoListItemRS>> ListarConsentimientosAsync(TycBaseContext context, DateTime? fecha, string estado, int empresaId,
        int usuarioId);
    Task<List<ConsentimientosRS>> ListarConsentimientosPorEmpresaAsync(TycBaseContext context, int? empresaId, DateTime? fechaInicial, 
        DateTime? fechaFinal, string estado, int usuarioId, string terminoBusqueda);
    Task<bool> EliminarConsentimientoAsync(TycBaseContext context, Guid id);
}

