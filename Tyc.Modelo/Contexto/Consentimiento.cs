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
        public int ConsConsecuencia { get; set; }
        
        [Column(Name = "empr_empr")]
        public int EmpresaId { get; set; }

        [Column(Name = "usua_usua")]
        public int? UsuaUsuario { get; set; }

        [Column(Name = "text_textterminosEmpresa")]
        public int? TextTerminosEmpresa { get; set; }

        [Column(Name = "text_textterminocompartirinfo")]
        public int? TgeTextTerminoCompartirInfo { get; set; }

        [Column(Name = "text_textofertas")]
        public int? TgeTextOfertas { get; set; }

        [Column(Name = "cons_nombre")]
        public string? ConsNombre { get; set; }

        [Column(Name = "cons_apellido")]
        public string? ConsApellido { get; set; }

        [Column(Name = "cons_email")]
        public string? ConsEmail { get; set; }

        [Column(Name = "cons_movil")]
        public string? ConsMovil { get; set; }

        [Column(Name = "cons_identificacion")]
        public string? ConsIdentificacion { get; set; }

        [Column(Name = "cons_fechacreacionconsentimient", IsDbGenerated = true)]
        public DateTime? ConsFechaCreacionConsentimiento { get; set; }

        [Column(Name = "cons_fechaaceptacionconsentimie")]
        public DateTime? ConsFechaAceptacionConsentimiento { get; set; }

        [Column(Name = "cons_aceptoterminosEmpresa")]
        public string? ConsAceptoTerminosEmpresa { get; set; }

        [Column(Name = "cons_aceptoterminoscompartirinf")]
        public string? ConsAceptoTerminosCompartirInfo { get; set; }

        [Column(Name = "cons_aceptoterminosrecibirofert")]
        public string? ConsAceptoTerminosRecibirOfertas { get; set; }

        [Column(Name = "cons_contactabilidadsms")]
        public string? ConsContactabilidadSms { get; set; }

        [Column(Name = "cons_contactabilidadwhatsappp")]
        public string? ConsContactabilidadWhatsapp { get; set; }

        [Column(Name = "cons_contactabilidademail")]
        public string? ConsContactabilidadEmail { get; set; }

        [Column(Name = "cons_contactabilidadmovil")]
        public string? ConsContactabilidadMovil { get; set; }

        [Column(Name = "cons_guid")]
        public Guid? ConsGuid { get; set; }

        [Column(Name = "cons_medio")]
        public string? ConsMedio { get; set; }
    }
}
