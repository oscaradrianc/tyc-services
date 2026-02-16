using Administrador.Modelo.Contexto;
using Administrador.Modelo.Tipos;
using Administrador.ServiceLogs.Auth;
using MapsterMapper;
using ServiceStack;
using ServiceStack.Configuration;
using Tyc.Interface.Request;
using Tyc.Interface.Response.General;
using Tyc.Interface.Response.Usuarios;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface;


public class UsuariosWS : Service
{
    private readonly IMapper _mapper;
    private readonly IUsuarioService _usuarioService;

    public UsuariosWS(
        IUsuarioService usuarioService,       
        IMapper mapper)
    {
        _mapper = mapper;
        _usuarioService = usuarioService;
    }

    [Authenticate]
    public ApiResponse<UsuarioRS> Post(CreateUsuarioRQ request)
    {
        // UserSession va por defecto           
        CustomUserSession userSession = SessionAs<CustomUserSession>();

        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Modelo.Contexto.Usuario>(request);

            var res = _usuarioService.CrearUsuario(dbSigo, entity);

            return res;
        }
    }

    [Authenticate]
    public ApiResponse<UsuarioRS> Put(UpdateUsuarioRQ request)
    {
        // UserSession va por defecto           
        CustomUserSession userSession = SessionAs<CustomUserSession>();
        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var entity = _mapper.Map<Modelo.Contexto.Usuario>(request);
            var res = _usuarioService.ActualizarUsuario(dbSigo, entity);
            return res;
        }
    }

    [Authenticate]
    public ChangePasswordRS Post(CambiarPasswordUsuarioRQ request)
    {
        CustomUserSession userSession = this.SessionAs<CustomUserSession>();
        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            ChangePasswordRQ changePassUserRQ = new ChangePasswordRQ { NewPassword = request.Password };

            var res = _usuarioService.CambiarClave(dbSigo, changePassUserRQ, userSession, this.Request.RemoteIp);
            return res;
        }
    }

    [Authenticate]
    public ApiResponse<bool> Post(EncriptarDefectoRQ request)
    {
        CustomUserSession userSession = this.SessionAs<CustomUserSession>();
        using (TycBaseContext dbSigo = TycContext.DataContext(userSession))
        {
            var res = _usuarioService.EncriptarPassDefecto(dbSigo, request.Id);
            return res;

        }
    }
}
