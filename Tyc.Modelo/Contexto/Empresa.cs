using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto
{
    [DataContract]
    [Table(Name = "tgen_empresas")]
    public class Empresa
    {
        [Column(Name = "empr_empr", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int EmpresaId { get; set; }

        [Column(Name = "empr_guid")]
        public Guid? Guid { get; set; }

        [Column(Name = "agent_name")]
        public string Nombre { get; set; }

        [Column(Name = "empr_ciudadEmpresa")]
        public string CiudadEmpresa { get; set; }

        [Column(Name = "empr_direccion")]
        public string Direccion { get; set; }

        [Column(Name = "empr_telefono")]
        public string Telefono { get; set; }

        [Column(Name = "empr_website")]
        public string Website { get; set; }

        [Column(Name = "empr_mailcontactos")]
        public string MailContactos { get; set; }

        [Column(Name = "empr_logoiso9000")]
        public string LogoIso9000 { get; set; }

        [Column(Name = "agent_nombrecontacto")]
        public string NombreContacto { get; set; }

        [Column(Name = "empr_maildelcontacto")]
        public string MailDelContacto { get; set; }

        [Column(Name = "empr_telcontacto")]
        public string TelContacto { get; set; }

        [Column(Name = "empr_subdominio")]
        public string Subdominio { get; set; }

        [Column(Name = "empr_logoiso27001")]
        public string LogoIso27001 { get; set; }

        [Column(Name = "empr_manejaterminosycondiciones")]
        public string ManejaTerminosYCondiciones { get; set; }

        [Column(Name = "empr_manejatyccompartirinfo")]
        public string ManejaTycCompartirInfo { get; set; }

        [Column(Name = "empr_manejatycrecibirofertas")]
        public string ManejaTycRecibirOfertas { get; set; }

        [Column(Name = "empr_contactabilidassms")]
        public string ContactabilidadSms { get; set; }

        [Column(Name = "empr_contactabilidasemail")]
        public string ContactabilidadEmail { get; set; }

        [Column(Name = "empr_contactabilidaswhatsapp")]
        public string ContactabilidadWhatsapp { get; set; }

        [Column(Name = "empr_contactabilidasmovil")]
        public string ContactabilidadMovil { get; set; }

        [Column(Name = "empr_solicitanombre")]
        public string SolicitaNombre { get; set; }

        [Column(Name = "empr_solicitaapellido")]
        public string SolicitaApellido { get; set; }

        [Column(Name = "empr_solicitaemail")]
        public string SolicitaEmail { get; set; }

        [Column(Name = "empr_solicitatelefono")]
        public string SolicitaTelefono { get; set; }

        [Column(Name = "empr_solicitaidentificacion")]
        public string SolicitaIdentificacion { get; set; }

        [Column(Name = "empr_Empresabloqueada")]
        public string Estado { get; set; }

        [Column(Name = "empr_manejacorporativo")]
        public string ManejaCorporativo { get; set; }

        [Column(Name = "empr_manejaconsolidacion")]
        public string ManejaConsolidacion { get; set; }

        [Column(Name = "empr_manejareceptivo")]
        public string ManejaReceptivo { get; set; }

        [Column(Name = "empr_manejamayoreo")]
        public string ManejaMayoreo { get; set; }

        [Column(Name = "empr_manejaeventos")]
        public string ManejaEventos { get; set; }

        [Column(Name = "empr_logobase64")]
        public string LogoBase64 { get; set; }
    }

    /// <summary>
    /// Constantes para campos SI/NO
    /// </summary>
    public static class OpcionSiNo
    {
        public const string Si = "SI";
        public const string No = "NO";
    }

    /// <summary>
    /// Estados de bloqueo de Empresa
    /// </summary>
    public static class EstadoBloqueoEmpresa
    {
        public const string Bloqueada = "BLOQUEADA";
        public const string NoBloqueada = "NO_BLOQUEADA";
    }
}