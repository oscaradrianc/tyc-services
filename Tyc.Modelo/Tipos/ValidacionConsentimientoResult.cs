using System.Collections.Generic;
using Tyc.Modelo.Contexto;

namespace Tyc.Modelo.Tipos
{
    public class ValidacionConsentimientoResult
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public Consentimiento Consentimiento { get; set; }
        public Empresa EmpresaConsentimiento { get; set; }
        public List<TipoIdentificacion> TiposIdentificacion { get; set; }
    }
}
