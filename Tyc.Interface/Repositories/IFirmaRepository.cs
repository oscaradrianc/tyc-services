using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}