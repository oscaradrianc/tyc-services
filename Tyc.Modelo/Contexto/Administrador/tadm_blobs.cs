using System.Runtime.Serialization;
using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto.Administrador
{
    [DataContract]
    [Table(Name = "tadm_blobs")]
    public class TadmBlobs
    {
        [Column(Name = "blob_blob", IsPrimaryKey = true)]
        public int IdBlob { get; set; }


        [Column(Name = "blob_binario")]
        public byte[] Binario { get; set; }

    }
}