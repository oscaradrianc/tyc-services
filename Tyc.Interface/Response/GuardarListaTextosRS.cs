using System.Collections.Generic;

namespace Tyc.Interface.Response
{
    public class GuardarListaTextosRS
    {
        public int Insertados { get; set; }
        public int Actualizados { get; set; }
        public int Versionados { get; set; }
        public List<int> IdsAfectados { get; set; }
    }
}
