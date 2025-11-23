using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto.Administrador
{
    [Table(Name = "v_adm_usuario")]
    public class vAdmUsuario
    {
        [Column(Name = "usua_usuario")]
        public int UsuaUsuario { get; set; }

        [Column(Name = "usua_nombre")]
        public string NombreUsuario { get; set;}

        [Column(Name = "usua_identificador")]
        public string IdentificadorLogin { get; set;}

        [Column(Name = "usua_email")]
        public string Email { get; set; }

        [Column(Name = "usua_documento")]
        public string NumDocumento { get; set; }
    }
      
}
