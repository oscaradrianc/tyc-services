using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto.General
{
    [DataContract]
    [Table(Name = "grl_tblob")]
    public class Blob
    {
        [Column(Name = "blob_blob", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdBlob { get; set; }
        [Column(Name = "empr_empresa")]
        public short IdEmpresa { get; set; }
        [Column(Name = "tipb_tipoblob")]
        public short TipoBlob { get; set; }
        [Column(Name = "blob_nombre")]
        public string Nombre { get; set; }
        [Column(Name = "blob_origen")]
        public string Origen { get; set; }
        [Column(Name = "blob_descripcion")]
        public string Descripcion { get; set; }
        [Column(Name = "blob_binario")]
        public byte[] Binario { get; set; }
        [Column(Name = "blob_version")]
        public string Version { get; set; }
        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }
        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }
    }
}
