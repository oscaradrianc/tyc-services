using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto.General
{
    [DataContract]
    [Table(Name = "grl_ttexto")]
    public class Texto
    {
        [Column(Name = "text_texto", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdTexto
        {
            get;
            set;
        }

        [Column(Name = "empr_empresa")]
        public short IdEmpresa
        {
            get;
            set;
        }

        [Column(Name = "text_nombre")]
        public string Nombre
        {
            get;
            set;
        }

        [Column(Name = "text_contenido")]
        public string Contenido
        {
            get;
            set;
        }

        [Column(Name = "logs_usuario")]
        public int? Usuario
        {
            get;
            set;
        }

        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion
        {
            get;
            set;
        }
    }
}