using System;
using System.Collections.Generic;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Repositories;

public interface IConsentimientoRepository
{
    Consentimiento GetById(TycBaseContext context, int id);
    Consentimiento GetByGuid(TycBaseContext context, Guid guid);
    Consentimiento CrearConsentimiento(TycBaseContext context, Consentimiento datosConsentimiento);
    bool ActualizarAceptaciones(TycBaseContext context, Guid consentimientoId, string medio,
    List<string> opcionesContactabilidad,
    Dictionary<string, int> politicasAceptadas,
    DateTime fechaAceptacion, string estado);
    bool Exists(TycBaseContext context, Guid id);
    TipoIdentificacion GetTipoIdentificacion(TycBaseContext context, int empresaId, int tipoDocumentoId);
    List<Consentimiento> ListarPorFiltros(TycBaseContext context, DateTime? fecha, string? estado);
    List<Consentimiento> ListarPorEmpresa(TycBaseContext context, int empresaId, DateTime? fecha, string? estado);
}

