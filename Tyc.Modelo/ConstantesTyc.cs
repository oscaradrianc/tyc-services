namespace Tyc.Modelo
{    public static class ConstantesTyc
    {
        public const string KeyRutaTemporalMapas = "RutaTemporalMapas";
        public const string KeyConexionPostgis = "ConexionPostgis";
        public const string KeyUrlMapServer = "UrlMapServer";
        public const string KeyRutaCacheTiles = "RutaCacheTiles";
        public const string KeyRutaMapCache = "RutaMapCache";
        public const string KeyUrlGeocode = "UrlGeocode";

        public const string KeyUsuarioWS = "UsuarioWS";
        public const string KeyClaveUsuarioWS = "ClaveWS";
        public const string KeyUrlWSNotificacionPush = "UrlWSNotificacionPush";
        public const string TipoNotificacionPushPerfil = "P";

        public const string methodPost = "POST";
        public const string ContentTypeJson = "application/json; charset=utf-8";

        public const string tipoEstadoCerrado = "C";
        public const string tipoInspeccionPreoperacion = "P";
        public const string tipoInspeccionBarrido = "B";
        public const string tipoInspeccionPostOperacion = "O";

        public const string TipoVerificado = "S";
        public const string TipoNoVerificado = "N";
        public const string TipoEstadoTransito = "T";

        public const string DespachoVacio = "No hay referencia de despacho.";

        public const string IdentificadorNombreBlobApp = "VersionApp";

        #region Consulta

        public const string ErrDireccion = "Dirección desconocida";
        public const string ErrRestriccionApi = "Restricción API Key";

        public const string kstrReferenciaNovedadPeso = "PESO";
        public const string kstrReferenciaSwitchMantenimiento = "switch_mantenimiento";

        #endregion

        #region Grupo Dispositivos
        public const string GrupoDispositivoTipoUsoComportamiento = "C";
        public const string GrupoDispositivoTipoUsoVisualizacion = "V";
        public const string GrupoDispositivoTipoUsoBarrido = "B";
        public const string GrupoDispositivoTipoUsoClus = "K";
        #endregion

        #region Redis
        public const string KeyRedisListaAVL = "RedisListaAVL";
        public const string KeyRedisServer = "RedisServerSigo";
        public const string KeyRedisServerBarrido = "ServerRedisBarrido";
        #endregion

        #region Estado Operacion

        public const string tipoServicioRecoleccion = "R";
        public const string tipoServicioGrandesGeneradores = "G";
        public const string tipoServicioBarrido = "B";

        #endregion

        public const string kstrDespachoTerreno = "T";

        #region Ruta
        public const string kstrTipoRutaRecoleccion = "R";
        public const string kstrTipoRutaBarrido = "B";
        public const string kstrTipoRutaClus = "C";
        public const string kstrNombreRutaTaller = "Carro Taller";
        public const string kstrServicoMovilizacion = "Movilización";
        public const string kstrTramoRecorrido = "REC";
        #endregion

        #region Cadenas de Conexion

        public const string NombreCadenaOle = "CadenaOLE";

        #endregion

        #region Referencias Tipos de Listas

        public static string ReferenciaTipoListaGeometria = "tipogeometria";
        public static string ReferenciaTipoListaProyeccion = "tipoproyeccion";
        public static string ReferenciaTipoListaAutorizacion = "tipoautorizacion";
        public static string ReferenciaTipoMotivoAutorizacion = "tipomotivoautorizacion";
        public static string TipoRutaRecoleccion = "R";

        #endregion

        #region Tipo Capa

        public const string CapaTipoComplementaria = "C";
        public const string CapaTipoGeocerca = "G";
        public const string CapaTipoRuta = "R";
        public const string CapaTipoPeaje = "J";

        public const string CapaTipoArchivo = "DBF";

        #endregion

        #region Geometria Capa

        public const string CapaGeometriaPunto = "P";
        public const string CapaGeometriaPoligono = "S";
        public const string MapFileTipoPunto = "POINT";
        public const string MapFileTipoPoligono = "POLYGON";

        #endregion

        #region Tipo Generacion Capa

        public const string CapaTipoGeneracionCartografica = "C";
        public const string CapaTipoGeneracionPunto = "P";
        public const string CapaTipoGeneracionPuntoDireccion = "D";

        #endregion

        #region Geocercas

        public const int GeometriaGeocerca = 4326;
        public const string ProyeccionMapas = "EPSG:4326";

        #endregion

        #region Excepciones

        public const string Rechazada = "R";

        #endregion

        #region Persona

        public const string TipoCargoConductor = "C";
        public const string TipoCargoTripulante = "T";
        public const string TipoCargoMecanico = "M";

        #endregion

        #region Novedad Operacion

        public const string TipoNovedadManual = "M";
        public const string TipoEstadoNovedadCerrado = "C";

        public const string EstadoNovedadManualActiva = "A";

        public const string ReferenciaNovedadCambioConductor = "CAMBIO_CONDUCTOR";
        public const string ReferenciaNovedadCambioTripulante = "CAMBIO_TRIPULANTE";
        public const string OrigenNovedadEdicion = "edicion";

        public const string TipoInspeccionOperativo = "O";

        #endregion

        #region Despacho

        public const string TipoDespachoBase = "B";
        public const string TipoEstadoTerreno = "T";
        public const string TipoEstadoFinalizado = "F";
        public const string TipoEstadoPendiente = "P";
        public const string TipoEstadoAnulado = "A";
        public const string CreoDespachoDesdeWeb = "W";
        public const string CreoDespachoDesdeWebEdicion = "E";
        public const string CreoDespachoDesdeMovil = "M";
        public const string StatusValidacionOK = "OK";
        public const string StatusValidacionERROR = "ERROR";
        public const string SitioDespachoTerreno = "T";

        public const string EstadoSolicitudDespacho = "D";

        public const string EstadoNovedadActiva = "A";

        public const string ObservacionObjetoVacio = "Objeto recibido esta vacío.";
        public const string ObservacionSalidaBaseAut = "Salida Generada con el despacho móvil";
        public const string LicenciaVencida = "La licencia de conducción del conductor seleccionado ha expirado o no se posee información.";

        public const string TipoNovedadTripulanteLiberado = "L";

        public const string ObservacionDespachoAutomatico = "Despacho Generado Automaticamente";
        public const string ObservacionDespachoMasivo = "Despacho Masivo";
        public const string ObservacionNovedadManual = "Novedad Manual";

        public const string ObservacionPostOperacional = "El presente vehículo no posee post operacional, por favor realizarlo anted de finalizar.";

        public const int SolicitudMantenimiento = 1;

        #endregion

        #region OpcionSiNo

        public const string OpcionSi = "S";
        public const string OpcionNo = "N";

        #endregion

        #region Operaciones servicio SIGAB

        public const string ConsultaSeguimientoPorMicroPorAse = "/consultas/consultarseguimientosxmicroxAse";
        public const string consultarCumplimientoRutasxAse = "/consultas/consultarCumplimientoRutasxAse";
        public const string ConsultaPlaneacionesPorAse = "/consultas/consultarPlaneacionesxAse";

        #endregion

        #region Operaciones servicio Mantenimiento

        public const string DireccionCrearSolicitudesMantenimiento = "/crearsolicitudes";

        #endregion

        #region Barrido

        public const string TipoBolsaBarredora = "B";

        public const string TipoBolsaDesarene = "D";

        public const string TipoBolsaJumbo = "J";

        public const string TipoBolsaAltalene = "A";

        #endregion

        #region Prejifos Cache

        public const string PrefijoEmpresa = "E";
        public const string PrefijoCapa = "C";
        public const string PrefijoTiles = "T";
        public const string PrefijoFuente = "F";

        #endregion

        #region Tipo Geocerca y Salida Ruta Barrido

        public const string GeocercaCircular = "C";
        public const string GeocercaPoligonal = "P";

        public const string TipoOrigenPeajeGeocerca = "G";
        public const string TipoOrigenPeajeManual = "M";

        #endregion

        #region Alarma

        public const string AlarmaTipoCorreo = "C";

        #endregion

        #region Info Actividad Operacion

        public const string EstadoActividadOperacionActivo = "A"; // (A:activa, P:procesada, C:cancelada)
        public const string EstadoActividadOperacionProcesado = "P";

        #endregion

        #region Tipos de Retorno de recorridos

        public const string TipoRetornoBase = "B";
        public const string TipoRetornoRuta = "R";
        public const string TipoRetornoNuevoViaje = "N";

        #endregion

        #region Tipos Autorizacion

        public const string TipoAutorizacionAutorizarRuta = "CR";
        public const string TipoAutorizacionFinalizarSinTanqueo = "ST";
        public const string TipoAutorizacionFinalizarRecorrido = "FR";
        public const string TipoAutorizacionAnularRecorrido = "AR";
        public const string TipoAutorizacionAutorizarSobrePeso = "SP";
        public const string TipoAutorizacionLevantarVerificacion = "AV";


        #endregion

        #region Ruta

        public const string ServicioMovilizacion = "Movilización";
        public const string RutaTaller = "Taller";
        public const string TramoRecorrido = "REC";
        public const string ReferenciaTramoDF = "DF";
        public const string ReferenciaTramobB = "B";

        #endregion

        #region Referencia Novedades

        public const string TituloNotificacionPush = "Notificación SIGO";
        public const string RefNovSalidaBase = "SALIDA_BASE";
        public const string RefNovEntradaBase = "ENTRADA_BASE";
        public const string RefNovSalidaDF = "SALIDA_DISPOSICION";
        public const string RefNovViolacionTiempo = "VIOLACION_TIEMPO";
        public const string RefNovVarada = "NOVEDAD_VARADA";
        public const string RefNovSalidaFrecuencia = "SALIDA_FRECUENCIA";
        public const string MsgVehiculoRechazado = "El vehículo {0}, ha sido rechazado.";

        #endregion

        #region Parametros Generales

        public const string ReferenciaServicioAprovechamiento = "Aprovechamiento";
        public const string ReferenciaServicioEstacionTransferencia = "EstTransferencia";
        public const string ReferenciaSwitchPesoRecorrido = "switch_pesorecorrido";
        public const string ReferenciaParametrosTracking = "parametrostracksuper";
        public const string ReferenciaUtcEmpresa = "utcempresa";
        public const string ReferenciaSwitchPostOperacional = "post_operacional";
        public const string ReferenciaSwitchPesoSalidaBase = "SwitchPesoSalidaBase";
        public const string ReferenciaVersionProgramacion = "version_prog";
        public const string ReferenciaVersionProgramacionBarrido = "version_prog_barr";
        
        #endregion

        #region Programacion

        public const int VerProgAntigua = 1;

        #endregion

        #region Preoperacional

        public const string statusPreoperacionalWEB = "W";

        #endregion

        #region Tipo Blob

        public const short TipoBlobImagen = 2;

        #endregion

        #region Colores

        public static string TipoColorRojo = "255 87 51";
        public static string TipoColorVerde = "42 168 37";

        #endregion

        #region Tramo

        public const string TipoTramoInicial = "I";
        public const string TipoTramoRetorno = "R";

        #endregion

        #region Combustible
        public const string ReferenciaProveedorTerpel = "terpel";
        #endregion

        #region AccionesTabla
        public const string AccionTablaInsertar = "I";
        public const string AccionTablaActualizar = "U";
        public const string AccionTablaEliminar = "D";
        #endregion

        #region Respuesta Servicio
        public const string kstrRespuestaOK = "OK";
        public const string kstrRespuestaError = "ERROR";
        #endregion
    }
}
