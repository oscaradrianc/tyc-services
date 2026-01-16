using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto
{
    [DataContract]
    [Table(Name = "tgen_usuarios")]
    public class Usuario
    {
        [Column(
            Name = "usua_usua",
            IsPrimaryKey = true,
            IsDbGenerated = true,
            AutoSync = AutoSync.OnInsert)]
        public int UsuaUsua { get; set; }

        [Column(Name = "empr_empr")]
        public int? EmpresaId { get; set; }

        [Column(Name = "usua_nombre")]
        public string UsuaNombre { get; set; }

        [Column(Name = "usua_apellido")]
        public string UsuaApellido { get; set; }

        [Column(Name = "usua_email")]
        public string UsuaEmail { get; set; }

        [Column(Name = "usua_movil")]
        public string UsuaMovil { get; set; }

        [Column(Name = "usua_password")]
        public string UsuaPassword { get; set; }

        [Column(Name = "usua_ultimologin")]
        public DateTime? UsuaUltimoLogin { get; set; }

        [Column(Name = "usua_estado")]
        public string UsuaEstado { get; set; }

        [Column(Name = "usua_cambiarclave")]
        public string UsuaCambiarClave { get; set; }

        [Column(Name = "usua_guid")]
        public Guid? UsuaGuid { get; set; }

        [Column(Name = "usua_identificacion")]
        public string UsuaIdentificacion { get; set; }

        [Column(Name = "usua_puedecrearconsentimientos")]
        public string UsuaPuedeCrearConsentimientos { get; set; }

        [Column(Name = "usua_puedecrearusuariosadmin")]
        public string UsuaPuedeCrearUsuariosAdmin { get; set; }

        [Column(Name = "usua_puedeconsultardatos")]
        public string UsuaPuedeConsultarDatos { get; set; }

        [Column(Name = "usua_essuperusuario")]
        public string UsuaEsSuperUsuario { get; set; }

        [Column(Name = "usua_login")]
        public string UsuaLogin { get; set; }

        [Column(Name = "usua_ultimocambioclave")]
        public DateTime? UsuaUltimoCambioClave { get; set; }

        [Column(Name = "usua_feccre", IsDbGenerated = true)]
        public DateTime? UsuaFechaCreacion { get; set; }
    }
}
