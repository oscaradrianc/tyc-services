using Tyc.Modelo.Contexto;
using Tyc.Modelo;

namespace Tyc.Interface.Repositories;

public interface IConsentimientoRepository
{
    Consentimiento CrearConsentimiento(TycBaseContext context, Consentimiento datosConsentimiento);
}

