using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "sig_tregistromapcache")]
    public class RegistroMapcache
    {
        [Column(Name = "empr_empresa", IsPrimaryKey = true)]
        public int IdEmpresa { get; set; }
        [Column(Name = "capa_capa", IsPrimaryKey = true)]
        public int IdCapa { get; set; }

        [Column(Name = "rema_nombrecapa")]
        public string NombreCapa { get; set; }

        [Column(Name = "rema_urlcapa")]
        public string UrlCapa { get; set; }

        [Column(Name = "rema_fuente")]
        public string Fuente { get; set; }

        [Column(Name = "rema_tile")]
        public string Tile { get; set; }
    }
}
