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
        public MotorBD Motor { get; set; }

        public Table<Consentimiento> Consentimientos { get; set; }

        public TycBaseContext(System.Data.IDbConnection connection) : base(connection) { }
        public TycBaseContext(string connectionString) : base(connectionString) { }




        [Provider(typeof(Devart.Data.PostgreSql.Linq.Provider.PgSqlDataProvider))]
        public class SigoContextPostgreSQL : TycBaseContext
        {
            public SigoContextPostgreSQL(System.Data.IDbConnection connection) : base(connection) { }
            public SigoContextPostgreSQL(string connectionString) : base(connectionString) { }
        }

        [Provider(typeof(Devart.Data.Oracle.Linq.Provider.OracleDataProvider))]
        public class SigoContextOracle : TycBaseContext
        {
            public SigoContextOracle(System.Data.IDbConnection connection) : base(connection) { }
            public SigoContextOracle(string connectionString) : base(connectionString) { }
        }

        [Provider(typeof(Devart.Data.SqlServer.Linq.Provider.SqlDataProvider))]
        public class SigoContextSqlServer : TycBaseContext
        {
            public SigoContextSqlServer(System.Data.IDbConnection connection) : base(connection) { }
            public SigoContextSqlServer(string connectionString) : base(connectionString) { }
        }

        public static class SigoContext
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
                    res = new SigoContextOracle(connectionString)
                    {
                        Motor = MotorBD.ORACLE
                    };
                }
                else if (motor == MotorBD.POSTGRESQL)
                {
                    PgSqlMonitor pgSqlMonitor = new PgSqlMonitor();
                    pgSqlMonitor.IsActive = true;
                    res = new SigoContextPostgreSQL(connectionString)
                    {
                        Motor = MotorBD.POSTGRESQL
                    };
                }
                else
                {
                    res = new SigoContextSqlServer(connectionString)
                    {
                        Motor = MotorBD.SQLSERVER
                    };
                }

                return res;
            }
        }
    }
}