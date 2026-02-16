using Administrador.Modelo.Contexto;
using Administrador.Modelo.Tipos;
using Administrador.ServiceLogs.Auth;
using AdministradorCore.Cifrar;
using General.Utilidades.Cache;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Configuration;
using System;
using System.Collections.Generic;
using Tyc.Interface.Repositories;
using Tyc.Interface.Response.General;
using Tyc.Interface.Response.Usuarios;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Usuarios
{
    public class UsuariosBL : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuariosBL> _logger;

        public UsuariosBL(
            ILogger<UsuariosBL> logger,
            IMapper mapper,
            IUsuarioRepository usuarioRepository)
        {

            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public ApiResponse<UsuarioRS> CrearUsuario(TycBaseContext context, Modelo.Contexto.Usuario usuario)
        {
            try
            {
                //Valida si ya existe el usuario
                var response = new ApiResponse<UsuarioRS>();
                var usuarioExistente = _usuarioRepository.GetByLogin(context, usuario.UsuaLogin);

                if (usuarioExistente != null)
                {
                    response.Success = false;
                    response.Mensaje = "Ya existe un usuario con el login ingresado.";                   
                    return response;
                }

                var usuarioAdminExistente = _usuarioRepository.GetByLoginAdmin(context, usuario.UsuaLogin);

                if (usuarioAdminExistente != null)
                {
                    response.Success = false;
                    response.Mensaje = "Ya existe un usuario con el login ingresado.";               
                    return response;
                }

                usuario.UsuaFechaCreacion = DateTime.Now;
                usuario.UsuaCambiarClave = "S";

                string claveCifrada = new BaseCifrado(Convert.ToDateTime(usuario.UsuaFechaCreacion).ToString("yyyyMMdd"))
                    .EncryptSHA512(usuario.UsuaLogin.ToLower());

                usuario.UsuaPassword = claveCifrada;
                int usuaId = _usuarioRepository.CrearUsuario(context, usuario);

                return new ApiResponse<UsuarioRS>
                {
                    Success = true,
                    Mensaje = "Usuario creado exitosamente.",
                    Data = new UsuarioRS { UsurioId = usuaId }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario.");
                return new ApiResponse<UsuarioRS>
                {
                    Success = false,
                    Mensaje = "Ocurrió un error al crear el usuario."            
                };
            }
        }

        public ApiResponse<UsuarioRS> ActualizarUsuario(TycBaseContext context, Modelo.Contexto.Usuario usuario)
        {
            try
            {
                //Valida si ya existe el usuario
                var response = new ApiResponse<UsuarioRS>();
                var usuarioExistente = _usuarioRepository.GetById(context, usuario.UsuaUsua);
                if (usuarioExistente == null)
                {
                    response.Success = false;
                    response.Mensaje = "No existe el usuario que se desea actualizar.";
                    return response;
                }
                int res = _usuarioRepository.ActualizarUsuario(context, usuario);
                return new ApiResponse<UsuarioRS>
                {
                    Success = res == 1,
                    Mensaje = (res == 0) ? "Error al actualizar el usuario." : "Usuario actualizado exitosamente.",
                    Data = new UsuarioRS { UsurioId = usuario.UsuaUsua }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario.");
                return new ApiResponse<UsuarioRS>
                {
                    Success = false,
                    Mensaje = "Ocurrió un error al actualizar el usuario."
                };
            }
        }

        public ChangePasswordRS CambiarClave(TycBaseContext context, ChangePasswordRQ pChangePassUserRQ, CustomUserSession customUserSession, string IP)
        {
            var resp = new ChangePasswordRS();
   
            try
            {
                // Control SQL Injection
                //Boolean checkInjection = false;
                //checkInjection = this.SqlCheckInjection(pChangePassUserRQ.Login) ? true : checkInjection;
                //checkInjection = this.SqlCheckInjection(pChangePassUserRQ.Password) ? true : checkInjection;

                /*if (checkInjection)
                {
                    resp.Res = "Error";
                    resp.ErrorMessage = "Error 010";
                    return resp;
                }*/

                /*if (pChangePassUserRQ.Login == null || pChangePassUserRQ.Password == null)
                {
                    resp.Res = "Error";
                    resp.ErrorMessage = "Error 000";
                    return resp;
                }*/
                // Validar Usuario
                Dictionary<string, object> lsql_User = null;
                /*lsql_User = dbAdm.ExecuteQuerySingle(string.Format("SELECT usua_login, usua_feccre, usua_nombres, usua_email  FROM tadm_usuarios"
                        + " WHERE usua_login = '{0}' and usua_estado ='A' and (usua_loginsso='N' or usua_loginsso is NULL)", pChangePassUserRQ.Login));*/
                var lsqlUser = _usuarioRepository.GetByLogin(context, customUserSession.UserName);

                if (lsqlUser == null)
                {
                    resp.Success = false;
                    resp.Error = "Error 001";
                    return resp;
                }


                string newPassword = pChangePassUserRQ.NewPassword?.Trim() ?? "";
                string login = lsqlUser.UsuaLogin?.ToString()?.Trim() ?? "";
                string nombres = lsqlUser.UsuaNombre?.ToString()?.Trim() ?? "";
                string email = lsqlUser.UsuaEmail?.ToString()?.Trim() ?? "";

                string passLower = newPassword.ToLower();

                // Verificar similitud con login, nombre o email
                if (!string.IsNullOrEmpty(login) && passLower.Contains(login.ToLower()))
                {
                    resp.Success = false;
                    resp.Error = "La contraseña no puede contener el nombre de usuario.";
                    return resp;
                }

                if (!string.IsNullOrEmpty(nombres) && passLower.Contains(nombres.ToLower()))
                {
                    resp.Success = false;
                    resp.Error = "La contraseña no puede contener el nombre del usuario.";
                    return resp;
                }

                if (!string.IsNullOrEmpty(email))
                {
                    string emailUser = email.Split('@')[0]; // Solo la parte antes del @
                    if (passLower.Contains(emailUser.ToLower()))
                    {
                        resp.Success = false;
                        resp.Error = "La contraseña no puede contener el correo del usuario.";
                        return resp;
                    }
                }

                // Genera Guid
                var Token = Guid.NewGuid().ToString();

                // Password encriptado
                string passwordR = new BaseCifrado(Convert.ToDateTime(lsqlUser.UsuaFechaCreacion).ToString("yyyyMMdd")).EncryptSHA512(pChangePassUserRQ.NewPassword);

                // Se Guarda el registro en bd
                /*var sqlQuery = string.Format("INSERT INTO tadm_usuacambio (" +
                    "reco_reco," +
                    "usua_login," +
                    "reco_fecha," +
                    "reco_estado," +
                    "reco_nuevaclave, " +
                    "reco_ip," +
                    "reco_vigencia)" +
                    "VALUES (" +
                    "'{0}'," +
                    "'{1}'," +
                    "CURRENT_TIMESTAMP," +
                    "'C'," +
                    "'{2}'," +
                    "'{3}'," +
                    "'{4}')",
                Token, pChangePassUserRQ.Login, passwordR, IP, vigencia);
                dbAdm.ExecuteQuerySingle(sqlQuery);*/

                ChangePasswordRQ pass = new ChangePasswordRQ();
                pass.NewPassword = pChangePassUserRQ.NewPassword;
                //ChangePasswordRS userChange = new ServiceAuth(appSettings, _logger).ChangePasswordRQ(dbAdm.customUserSession, pass);
                int userChange = _usuarioRepository.CambiarClave(context, lsqlUser.UsuaUsua, passwordR);

                if (userChange == 0)
                {
                    resp.Success = false;
                    resp.Error = "Error al cambiar la contraseña";
                }
                else
                {
                    LimpiarCacheRQ limpiarCacheRQ = new LimpiarCacheRQ();
                    List<string> res = HostContext.Cache.LimpiarCache(limpiarCacheRQ, customUserSession);                  
                }

                resp.Success = true;
                resp.Error = "";
                return resp;
            }
            catch (Exception e)
            {
                resp.Success = false;
                resp.Error = e.Message;
                return resp;
            }
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

        public Boolean SqlCheckInjection(string text)
        {
            bool isSQLInjection = false;

            if (String.IsNullOrWhiteSpace(text))
            {
                return isSQLInjection;
            }
            ;

            string[] sqlCheckList = {
            "--",
            ";--",
            ";",
            "/*",
            "*/",
            "@@",
            "@",
            "char",
            "nchar",
            "varchar",
            "nvarchar",
            "alter",
            "begin",
            "create",
            "cursor",
            "declare",
            "delete",
            "drop",
            "exec",
            "execute",
            "fetch",
            "insert",
            "kill",
            "select",
            "sys",
            "sysobjects",
            "syscolumns",
            "table",
            "update"
        };

            string CheckString = text.Replace("'", "''");

            for (int i = 0; i <= sqlCheckList.Length - 1; i++)
            {
                if ((CheckString.IndexOf(sqlCheckList[i], StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    isSQLInjection = true;
                }
            }

            return isSQLInjection;
        }


        public ApiResponse<bool> EncriptarPassDefecto(TycBaseContext context, int idUsuario)
        {
            try
            {
                //Valida si ya existe el usuario
                var response = new ApiResponse<bool>();
                var usuarioExistente = _usuarioRepository.GetById(context, idUsuario);

                if (usuarioExistente != null)
                {
                    string claveCifrada = new BaseCifrado(Convert.ToDateTime(usuarioExistente.UsuaFechaCreacion).ToString("yyyyMMdd"))
                    .EncryptSHA512(usuarioExistente.UsuaLogin.ToLower());

                    
                    int res = _usuarioRepository.ActualizarClave(context, usuarioExistente.UsuaUsua, claveCifrada);

                    return new ApiResponse<bool>
                    {
                        Success = res == 1,
                        Mensaje = (res == 0) ? "Error al actualizar la clave defecto." : "",
                        Data = true
                    };
                }

                return new ApiResponse<bool>
                {
                    Success = false,
                    Mensaje = "No existe el usuario",
                    Data = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la clave por defecto.");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Mensaje = "Ocurrió un error al actualizar la clave por defecto."
                };
            }
        }
    }
}
