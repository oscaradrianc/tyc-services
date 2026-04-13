using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

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
    public async Task<Firma> CreateAsync(TycBaseContext context, Firma entity)
    {
        context.GetTable<Firma>().InsertOnSubmit(entity);
        await Task.Run(() => context.SubmitChanges());
        return entity;
    }

    public async Task<Firma> GetByConsentimientoAsync(TycBaseContext context, int consentimientoId)
    {
        return await Task.Run(() => context.GetTable<Firma>()
            .FirstOrDefault(x => x.ConsConsecuencia == consentimientoId));
    }

    public async Task<bool> ExisteFirmaParaConsentimientoAsync(TycBaseContext context, int consentimientoId)
    {
         return await Task.Run(() => context.GetTable<Firma>()
            .Any(x => x.ConsConsecuencia == consentimientoId));
    }

    public async Task<bool> EliminarAsync(TycBaseContext context, int consentimientoId)
    {
        var firma = await GetByConsentimientoAsync(context, consentimientoId);
        if (firma == null)
            return false;

        context.GetTable<Firma>().DeleteOnSubmit(firma);
        await Task.Run(() => context.SubmitChanges());
        return true;
    }

    public async Task<List<Firma>> GetByConsentimientoIdsAsync(TycBaseContext context, List<int> consentimientoIds)
    {
        return await Task.Run(() =>
        {
            if (consentimientoIds == null || !consentimientoIds.Any())
                return new List<Firma>();

            var idsValidos = consentimientoIds.Where(id => id > 0).Distinct().ToList();

            if (!idsValidos.Any())
                return new List<Firma>();

            return context.GetTable<Firma>()
                .Where(x => idsValidos.Contains(x.ConsConsecuencia))
                .ToList();
        });
    }
}