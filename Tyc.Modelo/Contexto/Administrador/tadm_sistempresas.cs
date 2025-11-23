using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto.Administrador
{
    [Table(Name = "tadm_sistempresas")]
    public class SistEmpresas
    {
        [Column(Name = "sist_sist")]
        public int IdSistema { get; set; }

        [Column(Name = "empr_empr")]
        public int IdEmpresa { get; set; }

        [Column(Name = "empr_codigo")]
        public string CodigoEmpresa { get; set; }

        [Column(Name = "empr_nombre")]
        public string NombreEmpresa { get; set; }

        [Column(Name = "empr_servidor")]
        public string ServidorEmpresa { get; set; }

        [Column(Name = "empr_basedatos")]
        public string NombreBaseDatos { get; set; }

        [Column(Name = "empr_motor")]
        public string MotorBD { get; set; }

        [Column(Name = "empr_tipoconexion")]
        public string TipoConexion { get; set; }
    }
}
