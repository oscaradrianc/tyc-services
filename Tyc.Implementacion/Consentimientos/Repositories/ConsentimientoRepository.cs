using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Consentimientos.Repositories;

public class ConsentimientoRepository : IConsentimientoRepository
{
    public Consentimiento CrearConsentimiento(TycBaseContext context, Consentimiento consentimientoEntity)
    {     
            context.Consentimientos.InsertOnSubmit(consentimientoEntity);
            context.SubmitChanges();
            return consentimientoEntity;        
    }
}