using System;
using System.Collections.Generic;
using Tyc.Interface.Request;
using Tyc.Interface.Response;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using static Tyc.Interface.Request.ConsentimientoPublicoRQ;

namespace Tyc.Interface.Services;

public interface IConsentimientoService
{
    ConfirmacionConsentimientoRS ObtenerConfirmacionConsentimiento(TycBaseContext context, int id);
    int CrearConsentimiento(TycBaseContext context, Consentimiento entity);

    bool ActualizarConsentimientoConFirma(TycBaseContext context, ActualizarConsentimientoConFirma request);
    FormularioConsentimientoRS ObtenerFormularioConsentimiento(TycBaseContext context, string subdominio,
        string idEncriptado);

    bool ActualizarConsentimiento(TycBaseContext context, ActualizarConsentimiento request);

    List<ConsentimientoListItemRS> ListarConsentimientos(TycBaseContext context, DateTime? fecha, string? estado);
}

