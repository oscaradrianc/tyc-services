using Tyc.Interface.Request;
using Tyc.Interface.Response;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Services;

public interface IConsentimientoService
{
    ConfirmacionConsentimientoRS ObtenerConfirmacionConsentimiento(TycBaseContext context, int id);
    int CrearConsentimiento(TycBaseContext context, Consentimiento entity);

    bool ActualizarConsentimientoConFirma(TycBaseContext context, ActualizarConsentimientoConFirma request);
    FormularioConsentimientoRS ObtenerFormularioConsentimiento(TycBaseContext context, string subdominio,
        string idEncriptado); 
}

