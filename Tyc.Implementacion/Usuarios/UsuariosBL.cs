using Administrador.Modelo.Contexto;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyc.Implementacion.Consentimientos;
using Tyc.Interface.Repositories;
using Tyc.Interface.Response;
using Tyc.Interface.Services;

namespace Tyc.Implementacion.Usuarios
{
    public class UsuariosBL
    {
        private readonly IConsentimientoRepository _repository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IFirmaRepository _firmaRepository;
        private readonly ITextoRepository _textoRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<UsuariosBL> _logger;
        private readonly ITextoService _textoService;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IConfiguration _configuration;

        public UsuariosBL(
            IConsentimientoRepository consentimientoRepository,
            IFirmaRepository firmaRepository,
            ITextoRepository textoRepository,
            IEmpresaRepository empresaRepository,
            IEmailService emailService,
            ILogger<UsuariosBL> logger,
            IMapper mapper,
            ITextoService textoService,
            ITemplateRenderer templateRenderer,
            IConfiguration configuration,
            IUsuarioRepository usuarioRepository)
        {
            _repository = consentimientoRepository;
            _usuarioRepository = usuarioRepository;
            _firmaRepository = firmaRepository;
            _textoRepository = textoRepository;
            _empresaRepository = empresaRepository;
            _emailService = emailService;
            _logger = logger;
            _textoService = textoService;
            _templateRenderer = templateRenderer;
            _configuration = configuration;
        }

        /*
        public static ApiResponse<object> CambiarClave(int idUsuario, string claveActual, string claveNueva)
        {
            ApiResponse<object> response = new ApiResponse<object>();

            if (claveActual == null || claveNueva == null)
            {
                response.Success = false;
                response.Mensaje = "Clave actual o nueva no pueden ser nulas.";
                return response;
            }

            // Validaciones de nueva contraseña
            string valPassword = this.validacionesPassword(dbAdm, usuario, password);

            if (valPassword != "OK")
            {
                respuesta.Add(agregarNovedad(codSistema, empresa, valPassword));
                return respuesta;
            }

            string result = string.Empty;


            string sql = string.Format("select u.usua_validausuario, u.usua_feccre, u.usua_cifra from tadm_usuarios u" +
                " inner join tadm_usuaempresas tu on tu.usua_login  = u.usua_login " +
                " where u.usua_estado='A' and u.usua_login ='{0}' and tu.empr_empr = {1} and tu.sist_sist = {2}",
            usuario, empresa, codSistema);
            Dictionary<string, object> Usuario = null;
            Usuario = dbAdm.ExecuteQuery(sql).FirstOrDefault();

            DateTime fechaCreacion = (DateTime)Usuario["USUA_FECCRE"];



            passwordCifrado = ClaveUsuarioReal(dbAdm.Connection, codSistema, empresa, usuario, password, fechaCreacion, out bool validaUsuario);

            Framework.Admon.BE.StatusLogin result = new Framework.Admon.BE.StatusLogin();
            var keybytes = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["LlaveCifradoAES"].ToString());
            var iv = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["LlaveCifradoAES"].ToString());

            //var keybytes = Encoding.UTF8.GetBytes("7061737323313233");
            //var iv =       Encoding.UTF8.GetBytes("7061737323313233");

            //DECRYPT FROM CRIPTOJS
            var encrypted = Convert.FromBase64String(claveActual);

            string claveActualDescry = UtilidadesBL.DecryptStringAES(encrypted, keybytes, iv);

            encrypted = Convert.FromBase64String(claveNueva);

            string claveNuevaDescry = UtilidadesBL.DecryptStringAES(encrypted, keybytes, iv);

            _connectionAdminString = Settings.GetInstance().GetAppSetting(ConfiguracionSIGO.NombreCadenaConexionAdmin, true);
            var dbFactory = new OrmLiteConnectionFactory(_connectionAdminString);

            using (IDbConnection dba = dbFactory.Open())
            {
                Framework.Admon.BE.Usuario usuario = dba.Single<Framework.Admon.BE.Usuario>(string.Format("SELECT * FROM adm_tusuario WHERE usua_usuario = {0}", idUsuario));

                if (usuario != null)
                {
                    usuario.IdUsuario = idUsuario;

                    result = Framework.Admon.BLL.UsuarioBL.ActualizarClaveDesdeSigo(usuario, claveActualDescry, claveNuevaDescry);
                }
                else
                {
                    result.Status = ConfiguracionSIGO.StatusValidacionERROR;
                    result.Mensaje = "Id de usuario no existe.";
                }
            }

            return result;
        }

        private string validacionesPassword(AppAdmBaseContext dbAdm, string usuario, string password)
        {
            Dictionary<string, object> lsql_User = null;
            lsql_User = dbAdm.ExecuteQuery(string.Format("SELECT usua_login, usua_feccre, usua_nombres, usua_email  FROM tadm_usuarios"
                    + " WHERE usua_login = '{0}' and usua_estado ='A' and (usua_loginsso='N' or usua_loginsso is NULL)", usuario)).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(lsql_User["USUA_LOGIN"].ToString()))
            {
                return "El usuario no es valido";
            }

            string newPassword = password?.Trim() ?? "";
            string login = lsql_User["USUA_LOGIN"]?.ToString()?.Trim() ?? "";
            string nombres = lsql_User["USUA_NOMBRES"]?.ToString()?.Trim() ?? "";
            string email = lsql_User["USUA_EMAIL"]?.ToString()?.Trim() ?? "";

            string passLower = newPassword.ToLower();

            // Verificar similitud con login, nombre o email
            if (!string.IsNullOrEmpty(login) && passLower.Contains(login.ToLower()))
            {
                return "La contraseña no puede contener el nombre de usuario.";
            }

            if (!string.IsNullOrEmpty(nombres) && passLower.Contains(nombres.ToLower()))
            {
                return "La contraseña no puede contener el nombre del usuario.";
            }

            if (!string.IsNullOrEmpty(email))
            {
                string emailUser = email.Split('@')[0]; // Solo la parte antes del @
                if (passLower.Contains(emailUser.ToLower()))
                {
                    return "La contraseña no puede contener el correo del usuario.";
                }
            }

            if (string.IsNullOrWhiteSpace(password))
                return "La contraseña no puede estar vacía.";

            if (password.Length < 8)
                return "La contraseña debe tener al menos 8 caracteres.";

            if (password.Length > 15)
                return "La contraseña no debe superar los 15 caracteres.";

            if (!password.Any(char.IsLower))
                return "La contraseña debe contener al menos una letra minúscula.";

            if (!password.Any(char.IsUpper))
                return "La contraseña debe contener al menos una letra mayúscula.";

            string permitidos = "#!$&+-*";
            if (!password.Any(c => permitidos.Contains(c)))
                return $"La contraseña debe contener al menos uno de los siguientes caracteres especiales: {permitidos}";

            return "OK";
        }
        */
    }
}
