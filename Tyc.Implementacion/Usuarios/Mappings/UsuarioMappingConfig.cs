using DevExpress.XtraSpreadsheet.Commands;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyc.Interface.Request;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Usuarios.Mappings
{
    public class UsuarioMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateUsuarioRQ, Usuario>()
                //.Map(dest => dest.UsuaUsua, src => src.)
                .Map(dest => dest.EmpresaId, src => src.EMPR_EMPR)
                .Map(dest => dest.UsuaNombre, src => src.USUA_NOMBRE)
                .Map(dest => dest.UsuaApellido, src => src.USUA_APELLIDO)
                .Map(dest => dest.UsuaEmail, src => src.USUA_EMAIL)
                .Map(dest => dest.UsuaMovil, src => src.USUA_MOVIL)
                .Map(dest => dest.UsuaEstado, src => src.USUA_ESTADO)
                .Map(dest => dest.UsuaIdentificacion, src => src.USUA_IDENTIFICACION)
                .Map(dest => dest.UsuaLogin, src => src.USUA_LOGIN)
                .Map(dest => dest.UsuaPuedeCrearConsentimientos, src => src.USUA_PUEDECREARCONSENTIMIENTOS)
                .Map(dest => dest.UsuaPuedeConsultarDatos, src => src.USUA_PUEDECONSULTARDATOS)
                .Map(dest => dest.UsuaPuedeCrearUsuariosAdmin, src => src.USUA_PUEDECREARUSUARIOSADMIN)
                .Map(dest => dest.UsuaGuid, src => src.USUA_GUID);

            config.NewConfig<UpdateUsuarioRQ, Usuario>()
                .Map(dest => dest.UsuaUsua, src => src.USUA_USUA)
                .Map(dest => dest.EmpresaId, src => src.EMPR_EMPR)
                .Map(dest => dest.UsuaNombre, src => src.USUA_NOMBRE)
                .Map(dest => dest.UsuaApellido, src => src.USUA_APELLIDO)
                .Map(dest => dest.UsuaEmail, src => src.USUA_EMAIL)
                .Map(dest => dest.UsuaMovil, src => src.USUA_MOVIL)
                .Map(dest => dest.UsuaEstado, src => src.USUA_ESTADO)
                .Map(dest => dest.UsuaIdentificacion, src => src.USUA_IDENTIFICACION)
                .Map(dest => dest.UsuaLogin, src => src.USUA_LOGIN)
                .Map(dest => dest.UsuaPuedeCrearConsentimientos, src => src.USUA_PUEDECREARCONSENTIMIENTOS)
                .Map(dest => dest.UsuaPuedeConsultarDatos, src => src.USUA_PUEDECONSULTARDATOS)
                .Map(dest => dest.UsuaPuedeCrearUsuariosAdmin, src => src.USUA_PUEDECREARUSUARIOSADMIN)
                .Map(dest => dest.UsuaGuid, src => src.USUA_GUID);
        }
    }
}
