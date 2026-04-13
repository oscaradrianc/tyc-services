using System.Collections.Generic;
using System.Threading.Tasks;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Repositories;
public interface IFirmaRepository
{
    Firma Create(TycBaseContext context, Firma entity);
    Firma GetByConsentimiento(TycBaseContext context, int consentimientoId);
    bool ExisteFirmaParaConsentimiento(TycBaseContext context, int consentimientoId);
    bool Eliminar(TycBaseContext context, int consentimientoId);

    // Async Methods
    Task<Firma> CreateAsync(TycBaseContext context, Firma entity);
    Task<Firma> GetByConsentimientoAsync(TycBaseContext context, int consentimientoId);
    Task<bool> ExisteFirmaParaConsentimientoAsync(TycBaseContext context, int consentimientoId);
    Task<bool> EliminarAsync(TycBaseContext context, int consentimientoId);

    /// <summary>
    /// Obtiene firmas para múltiples consentimientos en una sola consulta
    /// </summary>
    Task<List<Firma>> GetByConsentimientoIdsAsync(TycBaseContext context, List<int> consentimientoIds);
}