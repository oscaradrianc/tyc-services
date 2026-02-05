using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tyc.Modelo;
using Tyc.Modelo.Consultas;
using Tyc.Modelo.Contexto;
using Tyc.Modelo.Tipos;

namespace Tyc.Interface.Repositories;

public interface IConsentimientoRepository
{
    Consentimiento GetById(TycBaseContext context, int id);
    Consentimiento GetByGuid(TycBaseContext context, Guid guid);
    Consentimiento CrearConsentimiento(TycBaseContext context, Consentimiento datosConsentimiento);
    bool ActualizarAceptaciones(TycBaseContext context, Guid consentimientoId, string medio,
    List<string> opcionesContactabilidad,
    Dictionary<string, int> politicasAceptadas,
    DateTime fechaAceptacion, string estado, string ipCliente, DatosCliente datosCliente);
    bool Exists(TycBaseContext context, Guid id);
    TipoIdentificacion GetTipoIdentificacion(TycBaseContext context, int empresaId, int tipoDocumentoId);
    List<TipoIdentificacion> GetTiposIdentificacion(TycBaseContext context, int empresaId);
    List<Consentimiento> ListarPorFiltros(TycBaseContext context, DateTime? fecha, string estado, int empresaId);
    List<ListaConsentimientos> ListarPorEmpresa(TycBaseContext context, int? empresaId, DateTime? fechaInicial, DateTime? FechaFinal, string estado);
    
    // Async Methods
    Task<Consentimiento> GetByIdAsync(TycBaseContext context, int id);
    Task<Consentimiento> GetByGuidAsync(TycBaseContext context, Guid guid);
    Task<Consentimiento> CrearConsentimientoAsync(TycBaseContext context, Consentimiento datosConsentimiento);
    Task<bool> ActualizarAceptacionesAsync(TycBaseContext context, Guid consentimientoId, string medio,
        List<string> opcionesContactabilidad,
        Dictionary<string, int> politicasAceptadas,
        DateTime fechaAceptacion, string estado, string ipCliente, DatosCliente datosCliente);
    Task<bool> ExistsAsync(TycBaseContext context, Guid id);
    Task<TipoIdentificacion> GetTipoIdentificacionAsync(TycBaseContext context, int empresaId, int tipoDocumentoId);
    Task<List<TipoIdentificacion>> GetTiposIdentificacionAsync(TycBaseContext context, int empresaId);
    Task<List<Consentimiento>> ListarPorFiltrosAsync(TycBaseContext context, DateTime? fecha, string estado, int empresaId);
    Task<List<ListaConsentimientos>> ListarPorEmpresaAsync(TycBaseContext context, int? empresaId, DateTime? fechaInicial, DateTime? FechaFinal, string estado);
}

