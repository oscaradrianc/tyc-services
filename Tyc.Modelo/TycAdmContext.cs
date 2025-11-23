using Administrador.Modelo.Contexto;
using Administrador.ServiceLogs.Auth;
using Devart.Data.Linq;
using Devart.Data.Linq.Mapping;
using Devart.Data.Oracle;
using Devart.Data.PostgreSql;
using Tyc.Modelo.Contexto.Administrador;
using Tyc.Modelo.Contexto.General;

namespace Tyc.Modelo
{
    public class SigoAdmBaseContext : DataContext
    {
        public SigoAdmBaseContext(System.Data.IDbConnection connection) : base(connection) { }
        public SigoAdmBaseContext(string connectionString) : base(connectionString) { }

        public CustomUserSession customUserSession { get; set; }
        public MotorBD Motor { get; set; }

        public Table<UsuarioCorreo> UsuariosCorreo;
        public Table<RegistroMapcache> RegistrosMapcache;
        public Table<SistEmpresas> SistEmpresas;
        public Table<Contexto.Administrador.ParamEmpresas> ParamEmpresas;
        public Table<TadmBlobs> TadmBlobs;
        public Table<TadmVerBlobs> TadmVerBlobs;
    }

    [Provider(typeof(Devart.Data.PostgreSql.Linq.Provider.PgSqlDataProvider))]
    public class SigoAdmContextPostgreSQL : SigoAdmBaseContext
    {
        public SigoAdmContextPostgreSQL(System.Data.IDbConnection connection) : base(connection) { }
        public SigoAdmContextPostgreSQL(string connectionString) : base(connectionString) { }
    }

    [Provider(typeof(Devart.Data.Oracle.Linq.Provider.OracleDataProvider))]
    public class SigoAdmContextOracle : SigoAdmBaseContext
    {
        public SigoAdmContextOracle(System.Data.IDbConnection connection) : base(connection) { }
        public SigoAdmContextOracle(string connectionString) : base(connectionString) { }
    }

    [Provider(typeof(Devart.Data.SqlServer.Linq.Provider.SqlDataProvider))]
    public class SigoAdmContextSqlServer : SigoAdmBaseContext
    {
        public SigoAdmContextSqlServer(System.Data.IDbConnection connection) : base(connection) { }
        public SigoAdmContextSqlServer(string connectionString) : base(connectionString) { }
    }

    public static class TycAdmContext
    {
        public static SigoAdmBaseContext DataContext(CustomUserSession customUserSession)
        {
            SigoAdmBaseContext res = DataContext(customUserSession.AppAdmConnectionString, customUserSession.MotorAppAdm);
            res.customUserSession = customUserSession;

            return res;
        }

        public static SigoAdmBaseContext DataContext(string connectionString, MotorBD motor)
        {
            SigoAdmBaseContext res;

            if (motor == MotorBD.ORACLE)
            {
                OracleMonitor oracleMonitor = new OracleMonitor();
                oracleMonitor.IsActive = true;
                res = new SigoAdmContextOracle(connectionString)
                {
                    Motor = MotorBD.ORACLE
                };
            }
            else if (motor == MotorBD.POSTGRESQL)
            {
                PgSqlMonitor pgSqlMonitor = new PgSqlMonitor();
                pgSqlMonitor.IsActive = true;
                res = new SigoAdmContextPostgreSQL(connectionString)
                {
                    Motor = MotorBD.POSTGRESQL
                };
            }
            else
            {
                res = new SigoAdmContextSqlServer(connectionString)
                {
                    Motor = MotorBD.SQLSERVER
                };
            }

            return res;
        }

    }
}
