using Devart.Data.Linq.Mapping;
using System;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "grl_tinfofamilia")]
    public class Familiar
    {
        [Column(Name = "infa_codigo", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdFamiliar { get; set; }

        [Column(Name = "pers_persona")]
        public int IdPersona { get; set; }

        [Column(Name = "info_numdocumento")]
        public int NroDocumento { get; set; }

        [Column(Name = "info_nombre")]
        public string Nombre { get; set; }

        [Column(Name = "info_tipoparentesco")]
        public string Parentesco { get; set; }

        [Column(Name = "info_fecnacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Column(Name = "info_telefonocontacto")]
        public string TelefonoContacto { get; set; }
    }
}
