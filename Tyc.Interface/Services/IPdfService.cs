using System;
using System.Threading.Tasks;
using Tyc.Modelo;

namespace Tyc.Interface.Services;

public interface IPdfService
{
    Task<byte[]> GenerarConsentimientoPdfAsync(TycBaseContext context, Guid consentimientoId, int textoId);
}