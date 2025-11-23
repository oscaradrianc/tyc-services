using Administrador.ServiceLogs.Auth;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using Tyc.Modelo.Request;
using Tyc.Modelo.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tyc.Modelo.TycBaseContext;

namespace Tyc.Interface
{
    public class TycWS : Service
    {
        public IAppSettings AppSettings { get; set; }
        public IConfiguration Configuration { get; set; }
        public static ILog _logger = LogManager.GetLogger(typeof(TycWS));
        private readonly IMapper _mapper;
        private readonly IConsentimientoRepository _repository;

        public TycWS(
        IConsentimientoRepository repository,
        IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<int>> Post(ConsentimientoRQ request)
        {       
            // UserSession va por defecto           
            CustomUserSession userSession = SessionAs<CustomUserSession>();

            var entity = _mapper.Map<Consentimiento>(request);

            // Mientras exista una sesion activa            
            //var response = await (dbSigo, dbAdmin, request);

            return response;
        }

    }
}
