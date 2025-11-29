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

    bool ActualizarAceptaciones(TycBaseContext context, int consentimientoId,
    List<string> opcionesContactabilidad,
    Dictionary<string, int> politicasAceptadas,
    DateTime fechaAceptacion);

    bool Exists(TycBaseContext context, int id);
}

