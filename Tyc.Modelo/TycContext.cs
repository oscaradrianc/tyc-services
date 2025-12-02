using Administrador.Modelo.Contexto;
using Administrador.ServiceLogs.Auth;
using Devart.Data.Linq;
using Devart.Data.Linq.Mapping;
using Devart.Data.Oracle;
using Devart.Data.PostgreSql;
using Tyc.Modelo.Contexto;


namespace Tyc.Modelo
{

    public class TycBaseContext : DataContext
    {
        public TycBaseContext(System.Data.IDbConnection connection) : base(connection) { }
        public TycBaseContext(string connectionString) : base(connectionString) { }

        public MotorBD Motor { get; set; }
        public Table<Consentimiento> Consentimientos { get; set; }
        public Table<Texto> Textos { get; set; }
        public Table<Empresa> Empresas { get; set; }
        public Table<Firma> Firmas { get; set; }
        public Table<TipoIdentificacion> TiposIdentificacion { get; set; }

    }


    [Provider(typeof(Devart.Data.PostgreSql.Linq.Provider.PgSqlDataProvider))]
    public class TycContextPostgreSQL : TycBaseContext
    {
        public TycContextPostgreSQL(System.Data.IDbConnection connection) : base(connection) { }
        public TycContextPostgreSQL(string connectionString) : base(connectionString) { }
    }

    [Provider(typeof(Devart.Data.Oracle.Linq.Provider.OracleDataProvider))]
    public class TycContextOracle : TycBaseContext
    {
        public TycContextOracle(System.Data.IDbConnection connection) : base(connection) { }
        public TycContextOracle(string connectionString) : base(connectionString) { }
    }

    [Provider(typeof(Devart.Data.SqlServer.Linq.Provider.SqlDataProvider))]
    public class TycContextSqlServer : TycBaseContext
    {
        public TycContextSqlServer(System.Data.IDbConnection connection) : base(connection) { }
        public TycContextSqlServer(string connectionString) : base(connectionString) { }
    }

    public static class TycContext
    {
        public static TycBaseContext DataContext(CustomUserSession customUserSession)
        {
            return DataContext(customUserSession.ProdConnectionString, customUserSession.MotorProd);
        }

        public static TycBaseContext DataContext(string connectionString, MotorBD motor)
        {
            TycBaseContext res;

            if (motor == MotorBD.ORACLE)
            {
                OracleMonitor oracleMonitor = new OracleMonitor();
                oracleMonitor.IsActive = true;
                res = new TycContextOracle(connectionString)
                {
                    Motor = MotorBD.ORACLE
                };
            }
            else if (motor == MotorBD.POSTGRESQL)
            {
                PgSqlMonitor pgSqlMonitor = new PgSqlMonitor();
                pgSqlMonitor.IsActive = true;
                res = new TycContextPostgreSQL(connectionString)
                {
                    Motor = MotorBD.POSTGRESQL
                };
            }
            else
            {
                res = new TycContextSqlServer(connectionString)
                {
                    Motor = MotorBD.SQLSERVER
                };
            }

            return res;
        }
    }
    
}