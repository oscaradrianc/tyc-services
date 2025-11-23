using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Devart.Data.Linq.Mapping;
using ServiceStack.DataAnnotations;

namespace Tyc.Modelo.Contexto.Administrador
{
    [DataContract]
    [Table(Name = "tadm_verblobs")]
    public class TadmVerBlobs
    {
        [Column(Name = "blob_blob", IsPrimaryKey = true)]
        public int IdBlob { get; set; }


        [Column(Name = "blob_nombre")]
        public string Nombre { get; set; }

        [Column(Name = "blob_descri")]
        public string Descripcion { get; set; }

        [Column(Name = "para_blob6")]
        public int ParaBlob { get; set; }

        [Column(Name = "blob_version")]
        public string Version { get; set; }

        [Column(Name = "blob_actini")]
        public string Actini { get; set; }

        [Column(Name = "blob_carpeta")]
        public string Carpeta { get; set; }

        [Column(Name = "blob_comprimido")]
        public string Comprimido { get; set; }

        [Column(Name = "blob_tamano")]
        public double Tamanio { get; set; }
    }
}