using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto
{
    [DataContract]
    [Table(Name = "tgen_textos")]
    public class Texto
    {
        [Column(Name = "text_text", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int TextText { get; set; }

        [Column(Name = "empr_empr")]
        public int EmpresaId { get; set; }

        [Column(Name = "usua_usua")]
        public int? UsuaUsuario { get; set; }

        [Column(Name = "text_tipotexto")]
        public string TextTipoTexto { get; set; }

        [Column(Name = "text_textodelosterminos")]
        public string TextTextoDelosTerminos { get; set; }

        [Column(Name = "text_estado")]
        public string TextEstado { get; set; }

        [Column(Name = "text_fechaccreacion", IsDbGenerated = true)]
        public DateTime TextFechaCreacion { get; set; }
    }

    /// <summary>
    /// Tipos de texto permitidos
    /// </summary>
    public static class TipoTexto
    {
        public const string TerminosEmpresa = "TERMINOS_Empresa";
        public const string TerminosCompartirDatos = "TERMINOS_COMPARTIRDATOS";
        public const string TerminosOfertas = "TERMINOS_OFERTAS";
    }

    /// <summary>
    /// Estados de texto
    /// </summary>
    public static class EstadoTexto
    {
        public const string Activo = "A";
        public const string Inactivo = "I";
    }
}