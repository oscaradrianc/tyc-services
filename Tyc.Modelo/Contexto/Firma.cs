using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto
{
    [DataContract]
    [Table(Name = "tgen_firmas")]
    public class Firma
    {
        [Column(Name = "firm_firm", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int FirmFirm { get; set; }

        [Column(Name = "cons_cons")]
        public int ConsConsecuencia { get; set; }

        [Column(Name = "firm_blob")]
        public byte[] FirmBlob { get; set; }

        [Column(Name = "firm_fechacreacion")]
        public DateTime FirmFechaCreacion { get; set; }
    }
}