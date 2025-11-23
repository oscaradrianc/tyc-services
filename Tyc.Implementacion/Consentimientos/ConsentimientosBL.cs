using Administrador.ServiceLogs.Auth;
using ServiceStack.Configuration;
using Tyc.Modelo;
using Tyc.Modelo.Request;
using Tyc.Modelo.Response;
using System;
using System.Linq;



namespace Tyc.Implementacion.Consentimientos
{
    public class ConsentimientosBL
    {
        public IAppSettings AppSettings { get; set; }

        public ConsentimientosBL(IAppSettings appSettings)
        {
            this.AppSettings = appSettings;
        }

        public ApiResponse<int> CrearConsentimiento(TycBaseContext dbSigo, 
            SigoAdmBaseContext dbAdmin, ConsentimientoRQ consentimientoRQ, CustomUserSession userSession)
        {

            try
            {
                dbSigo.Connection.Open();
                dbSigo.Transaction = dbSigo.Connection.BeginTransaction();

                DateTime fechaHoraActual = dbSigo.ExecuteQuery<DateTime>("SELECT CURRENT_TIMESTAMP").Single();

                //Consentimiento consentimiento = _mapper.Map<Consentimiento>(command.Request);
                return new ApiResponse<int>{ Data=1};

            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                if (dbSigo.Transaction != null)
                {
                    dbSigo.Transaction.Dispose();
                }
                if (dbSigo.Connection.State == System.Data.ConnectionState.Open)
                {
                    dbSigo.Connection.Close();
                }
            }
        }

    }
}
