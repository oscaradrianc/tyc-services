using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto
{
    [DataContract]
    [Table(Name = "tgen_consentimientos")]
    public class Consentimiento
    {
        [Column(Name = "cons_cons", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }
        
        [Column(Name = "empr_empr")]
        public int EmpresaId { get; set; }

        [Column(Name = "usua_usua")]
        public int UsuarioId { get; set; }

        [Column(Name = "text_texttyc")]
        public int? TerminosEmpresaId { get; set; }

        [Column(Name = "text_textcompartirinfo")]
        public int? CompartirInfoId { get; set; }

        [Column(Name = "text_textrecibirofertas")]
        public int? RecibirOfertasId { get; set; }

        [Column(Name = "cons_nombre")]
        public string NombreCliente { get; set; }

        [Column(Name = "cons_apellido")]
        public string ApellidoCliente { get; set; }

        [Column(Name = "cons_email")]
        public string EmailCliente { get; set; }

        [Column(Name = "cons_movil")]
        public string MovilCliente { get; set; }

        [Column(Name = "cons_identificacion")]
        public string IdentificacionCliente { get; set; }

        [Column(Name = "cons_fechacrea", IsDbGenerated = true)]
        public DateTime FechaCreacion { get; set; }

        [Column(Name = "cons_fechaaceptacion")]
        public DateTime? FechaAceptacion { get; set; }

        [Column(Name = "cons_aceptotyc")]
        public string AceptoTYC { get; set; }

        [Column(Name = "cons_aceptocompartirinfo")]
        public string AceptoCompartirInfo { get; set; }

        [Column(Name = "cons_aceptorecibirofertas")]
        public string AceptoRecibirOfertas { get; set; }

        [Column(Name = "cons_contactabilidadsms")]
        public string ContactabilidadSms { get; set; }

        [Column(Name = "cons_contactabilidadwhatsappp")]
        public string ContactabilidadWhatsapp { get; set; }

        [Column(Name = "cons_contactabilidademail")]
        public string ContactabilidadEmail { get; set; }

        [Column(Name = "cons_contactabilidadmovil")]
        public string ContactabilidadMovil { get; set; }

        [Column(Name = "cons_guid")]
        public Guid GuId { get; set; }

        [Column(Name = "cons_medio")]
        public string MedioAceptacion { get; set; }

        [Column(Name = "clas_tipoidentificacion1")]
        public int? TipoIdentificacion1 { get; set; }
        [Column(Name = "cons_estado")]
        public string Estado { get; set; }
    }
}
