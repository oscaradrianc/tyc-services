using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using System.Linq;

namespace Tyc.Implementacion.Firmas.Repositories;

public class FirmaRepository : IFirmaRepository
{
    public Firma Create(TycBaseContext context, Firma entity)
    {
        context.GetTable<Firma>().InsertOnSubmit(entity);
        context.SubmitChanges();
        return entity;
    }

    public Firma GetByConsentimiento(TycBaseContext context, int consentimientoId)
    {
        return context.GetTable<Firma>()
            .FirstOrDefault(x => x.ConsConsecuencia == consentimientoId);
    }

    public bool ExisteFirmaParaConsentimiento(TycBaseContext context, int consentimientoId)
    {
        return context.GetTable<Firma>()
            .Any(x => x.ConsConsecuencia == consentimientoId);
    }

    public bool Eliminar(TycBaseContext context, int consentimientoId)
    {
        var firma = GetByConsentimiento(context, consentimientoId);
        if (firma == null)
            return false;

        context.GetTable<Firma>().DeleteOnSubmit(firma);
        context.SubmitChanges();
        return true;
    }
}