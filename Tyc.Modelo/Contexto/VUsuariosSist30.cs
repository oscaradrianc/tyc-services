using System;
using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto
{
    [Table(Name = "public.v_usuarios_sist30")]
    public class VUsuariosSist30
    {
        [Column(Name = "usua_login", CanBeNull = false, IsPrimaryKey = true)]
        public string UsuaLogin { get; set; }

        [Column(Name = "usua_identifica")]
        public string Identifica { get; set; }

        [Column(Name = "usua_nombres")]
        public string Nombres { get; set; }

        [Column(Name = "usua_direccion")]
        public string Direccion { get; set; }

        [Column(Name = "usua_telefonos")]
        public string Telefonos { get; set; }

        [Column(Name = "usua_email")]
        public string Email { get; set; }

        [Column(Name = "usua_id")]
        public int? UsuaId { get; set; }

        [Column(Name = "usua_estado")]
        public string Estado { get; set; }

        [Column(Name = "usua_ultimoacceso")]
        public DateTime? UltimoAcceso { get; set; }

        [Column(Name = "usua_guid")]
        public string Guid { get; set; }

    }
}