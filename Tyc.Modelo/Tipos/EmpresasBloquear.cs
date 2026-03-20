using System;

namespace Tyc.Modelo.Tipos
{
    public class EmpresasBloquear
    {
        public int IdEmpresa { get; set; }
        public DateTime FechaLimiteEncuesta { get; set; }
        public bool Bloquear { get; set; }
    }
}
