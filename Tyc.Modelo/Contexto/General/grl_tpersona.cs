using Devart.Data.Linq.Mapping;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "grl_tpersona")]
    public class Persona
    {
        [Column(Name = "pers_persona", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdPersona { get; set; }

        [Column(Name = "empr_empresa")]
        public int IdEmpresa { get; set; }

        [Column(Name = "pers_nombre")]
        public string Nombre { get; set; }

        [Column(Name = "pers_tipodocumento")]
        public string TipoDocumento { get; set; }

        [Column(Name = "pers_numdocumento")]
        public decimal? Documento { get; set; }

        [Column(Name = "pers_fecnacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Column(Name = "dipo_sitionacimiento")]
        public int? SitioNacimiento { get; set; }

        [Column(Name = "pers_direccion")]
        public string Direccion { get; set; }

        [Column(Name = "pers_telefono")]
        public string Telefono { get; set; }

        [Column(Name = "pers_tipogenero")]
        public string TipoGenero { get; set; }

        [Column(Name = "pers_fax")]
        public string Fax { get; set; }

        [Column(Name = "pers_email")]
        public string Email { get; set; }

        [Column(Name = "pers_tipocargo")]
        public string TipoCargo { get; set; }

        [Column(Name = "pers_tipoeps")]
        public string TipoEps { get; set; }

        [Column(Name = "pers_contactonombre")]
        public string ContactoNombre { get; set; }

        [Column(Name = "pers_contactotelefono")]
        public string ContactoTelefono { get; set; }

        [Column(Name = "pers_tipoarp")]
        public string TipoArp { get; set; }

        [Column(Name = "pers_tiposangre")]
        public string TipoSangre { get; set; }

        [Column(Name = "pers_fecingreso")]
        public DateTime? FechaIngreso { get; set; }

        [Column(Name = "pers_iniciovacaciones")]
        public DateTime? InicioVacaciones { get; set; }

        [Column(Name = "pers_finvacaciones")]
        public DateTime? FinVacaciones { get; set; }

        [Column(Name = "pers_estado")]
        public string Estado { get; set; }

        [Column(Name = "pers_tipoestadocivil")]
        public string EstadoCivil { get; set; }

        [Column(Name = "pers_conyugenombre")]
        public string ConyugeNombre { get; set; }

        [Column(Name = "pers_conyugeocupacion")]
        public string ConyugeOcupacion { get; set; }

        [Column(Name = "pers_conyugetrabaja")]
        public string ConyugeTrabaja { get; set; }

        [Column(Name = "pers_tipocontrato")]
        public string TipoContrato { get; set; }

        [Column(Name = "pers_tipocontratista")]
        public string TipoContratista { get; set; }

        [Column(Name = "pers_tipoafp")]
        public string TipoAfp { get; set; }

        [Column(Name = "pers_tipocajacompensacion")]
        public string TipoCajaCompensacion { get; set; }

        [Column(Name = "pers_alias")]
        public string Alias { get; set; }

        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }

        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }

        [Column(Name = "pers_tiporeligion")]
        public string TipoReligion { get; set; }

        [Column(Name = "pers_tipogradoescolaridad")]
        public string TipoEscolaridad { get; set; }

        [Column(Name = "pers_tipoprofesion")]
        public string TipoProfesion { get; set; }

        [Column(Name = "dipo_divisionpolitica")]
        public int? MunicipioLabora { get; set; }

        [Column(Name = "pers_salario")]
        public int? Salario { get; set; }

        [Column(Name = "pers_tipoturno")]
        public string Turno { get; set; }

        [Column(Name = "pers_tipoarea")]
        public string Area { get; set; }

        [Column(Name = "pers_tipozona")]
        public string Zona { get; set; }

        [Column(Name = "pers_tipoactvidad")]
        public string TipoActividad { get; set; }

        [Column(Name = "pers_jefeinmediato")]
        public int? JefeInmediato { get; set; }

        [Column(Name = "pers_tallacamisa")]
        public short? TallaCamisa { get; set; }

        [Column(Name = "pers_tallapantalon")]
        public short? TallaPantalon { get; set; }

        [Column(Name = "pers_tallazapatos")]
        public short? TallaZapatos { get; set; }

        [Column(Name = "pers_gorra")]
        public string UsaGorra { get; set; }

        [Column(Name = "pers_tipovivienda")]
        public string TipoVivienda { get; set; }

        [Column(Name = "pers_tipoestrato")]
        public string TipoEstrato { get; set; }

        [Column(Name = "pers_estatura")]
        public short? Estatura { get; set; }

        [Column(Name = "pers_deportepractica")]
        public string NombreDeporte { get; set; }

        [Column(Name = "pers_novedad")]
        public string TipoNovedad { get; set; }

        [Column(Name = "pers_lugarreubicacion")]
        public string LugarReubicacion { get; set; }

        [Column(Name = "pers_barrioresidencia")]
        public string BarrioResidencia { get; set; }

        [Column(Name = "pers_municipioresidencia")]
        public int? MunicipioResidencia { get; set; }

        [Column(Name = "pers_cursoalturas")]
        public string CursoAlturas { get; set; }

        [Column(Name = "pers_cursorespelig")]
        public string CursoResiduos { get; set; }

        [Column(Name = "eqgp_equipogps")]
        public int? EquipoGPS { get; set; }

        [Column(Name = "blob_foto")]
        public int? IdBlobFoto { get; set; }

        [Column(Name = "pers_apellido1")]
        public string PrimerApellido { get; set; }

        [Column(Name = "pers_apellido2")]
        public string SegundoApellido { get; set; }

        [Column(Name = "pers_nombre1")]
        public string PrimerNombre { get; set; }

        [Column(Name = "pers_nombre2")]
        public string SegundoNombre { get; set; }

        [Ignore]
        public List<Familiar> InfoFamiliarList { get; set; }

        [Ignore]
        public List<ContratoTrabajo> InfoContratosList { get; set; }

        [Ignore]
        public List<NovedadPersonal> InfoNovedadPersonalList { get; set; }

        [Ignore]
        public List<CapacitacionConductor> InfoCapacitacionPersonalList { get; set; }
    }
}
