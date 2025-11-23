using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "tadm_usuarios")]
    public class UsuarioCorreo
    {
        [Column(Name = "usua_login", IsPrimaryKey = true, IsDbGenerated = false)]
        public string Login { get; set; }
        [Column(Name = "usua_identifica")]
        public string Identificacion { get; set; }
        [Column(Name = "usua_id")]
        public int IdUsuario{ get; set; }

        [Column(Name = "usua_email")]
        public string Email{ get; set; }

        [Column(Name = "usua_nombres")]
        public string Nombre{ get; set; }

        [Column(Name = "usua_estado")]
        public string Estado { get; set; }
    }
}
