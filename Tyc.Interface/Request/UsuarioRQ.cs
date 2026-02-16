using Administrador.Modelo.Tipos;
using ServiceStack;
using System;
using Tyc.Interface.Response.General;
using Tyc.Interface.Response.Usuarios;

namespace Tyc.Interface.Request;

[Route("/usuarios", "POST")]
public class CreateUsuarioRQ : IReturn<ApiResponse<UsuarioRS>>
{
    public int? USUA_USUA { get; set; }
    public int EMPR_EMPR { get; set; }
    public string USUA_NOMBRE { get; set; }
    public string USUA_APELLIDO { get; set; }
    public string USUA_EMAIL { get; set; }
    public string USUA_MOVIL { get; set; }
    public string USUA_PASSWORD { get; set; }
    public DateTime? USUA_ULTIMOLOGIN { get; set; }
    public string USUA_ESTADO { get; set; }
    public string USUA_CAMBIARCLAVE { get; set; }
    public Guid USUA_GUID { get; set; }
    public string USUA_IDENTIFICACION { get; set; }
    public string USUA_PUEDECREARCONSENTIMIENTOS { get; set; }
    public string USUA_PUEDECREARUSUARIOSADMIN { get; set; }
    public string USUA_PUEDECONSULTARDATOS { get; set; }
    public string USUA_ESSUPERUSUARIO { get; set; }
    public string USUA_LOGIN { get; set; }
    public DateTime? USUA_ULTIMOCAMBIOCLAVE { get; set; }
    public DateTime? USUA_FECCRE { get; set; }
}

[Route("/usuarios", "PUT")]
public class UpdateUsuarioRQ : IReturn<ApiResponse<UsuarioRS>>
{
    public int USUA_USUA { get; set; }
    public int EMPR_EMPR { get; set; }
    public string USUA_NOMBRE { get; set; }
    public string USUA_APELLIDO { get; set; }
    public string USUA_EMAIL { get; set; }
    public string USUA_MOVIL { get; set; }
    public string USUA_PASSWORD { get; set; }
    public DateTime? USUA_ULTIMOLOGIN { get; set; }
    public string USUA_ESTADO { get; set; }
    public string USUA_CAMBIARCLAVE { get; set; }
    public Guid USUA_GUID { get; set; }
    public string USUA_IDENTIFICACION { get; set; }
    public string USUA_PUEDECREARCONSENTIMIENTOS { get; set; }
    public string USUA_PUEDECREARUSUARIOSADMIN { get; set; }
    public string USUA_PUEDECONSULTARDATOS { get; set; }
    public string USUA_ESSUPERUSUARIO { get; set; }
    public string USUA_LOGIN { get; set; }
    public DateTime? USUA_ULTIMOCAMBIOCLAVE { get; set; }
    public DateTime? USUA_FECCRE { get; set; }
}

[Route("/Usuarios/ChangePass")]
public class CambiarPasswordUsuarioRQ : IReturn<ChangePasswordRS>
{
    public string Login { get; set; }
    public string Password { get; set; }

}


[Route("/Usuarios/encriptardefecto")]
public class EncriptarDefectoRQ : IReturn<ApiResponse<bool>>
{
    public int Id { get; set; }
}


