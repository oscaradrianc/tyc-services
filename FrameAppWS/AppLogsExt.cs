using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using ServiceStack.Logging;
using System.IO;
using System.Reflection;
using System;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;

namespace AdministradorCore.BaseHost
{
    public class LogsSerilog
    {
        // Switch para controlar el nivel dinámicamente
        public LoggingLevelSwitch LevelSwitch = new LoggingLevelSwitch();

        // Guardamos el nivel original definido en appsettings para poder restaurarlo
        public static LogEventLevel OriginalConfigLevel { get; private set; }

        /***
         * Configuración del Serilog
         */
        public static void ConfigureLogging(IConfiguration configuration, string env)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string nombreApp = configuration.GetValue<string>("NombreApp");

            // Obtenemos y guardamos el nivel original de appsettings
            var levelFromConfig = configuration.GetValue<string>("Serilog:MinimumLevel:Default");
            if (Enum.TryParse<LogEventLevel>(levelFromConfig, true, out var parsedLevel))
            {
                LoggingService.LevelSwitch.MinimumLevel = parsedLevel;
            }
            else
            {
                LoggingService.LevelSwitch.MinimumLevel = LogEventLevel.Error;
            }
            string GenerarNombreIndex()
            {
                string baseNombre = nombreApp ?? Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-");
                string envNormalizado = environment?.ToLower().Replace(".", "-") ?? "unknown";
                string fechaActual = DateTime.UtcNow.ToString("yyyy-MM-d");
                return $"{baseNombre}-{envNormalizado}-{fechaActual}";
            }

            string NombreIndex = GenerarNombreIndex();
            string config = configuration.GetValue<string>("ELASTIC_HOST");

            var loggerConfiguration = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .Enrich.WithCorrelationId()
              .Enrich.WithProperty("Environment", environment)
              .Enrich.WithProperty("ApplicationName", nombreApp ?? Assembly.GetExecutingAssembly().GetName().Name)
              .Enrich.WithProperty("ApplicationVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString())
              //.Enrich.WithProcessId()
              .Enrich.WithThreadId()
              .MinimumLevel.ControlledBy(LoggingService.LevelSwitch);

            loggerConfiguration.Filter.With(MultiConditionalFilter.CondicionesLog());
            loggerConfiguration.Filter.ByExcluding(MultiConditionalFilter.ExcluirMensajesNoDeseados());

            // loggerConfiguration.WriteTo.File(
            //    "logs/frameWs-.txt",
            //    rollingInterval: RollingInterval.Day,
            //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {CorrelationId} {UserId} {Message:lj}{NewLine}{Exception}"
            //);

            if (!string.IsNullOrEmpty(config))
            {
                Uri uri = new Uri(config);
                string[] credentials = uri.UserInfo.Split(':');

                loggerConfiguration.WriteTo.Elasticsearch(
                    ConfigureElasticSink(environment, uri, credentials, NombreIndex)
                );

                Console.WriteLine("Registro de Log de archivo y Conectando a Elasticsearch");
            }
            else
            {
                var conf_log = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                loggerConfiguration.ReadFrom.Configuration(configuration);
                Console.WriteLine("Registro de Log de archivo");
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        /**
         * Configuración de Elastic
         */
        static ElasticsearchSinkOptions ConfigureElasticSink(string environment, Uri uri, string[] credentials, string NombreIndex)
        {
            string scheme = uri.Scheme;
            string host = uri.Host;
            var elasticUri = new Uri($"{scheme}://{host}");

            var options = new ElasticsearchSinkOptions(elasticUri)
            {
                AutoRegisterTemplate = true,
                IndexFormat = NombreIndex,
                TemplateCustomSettings = new Dictionary<string, string>
                {
                    { "index.lifecycle.name", "sg_politica_limpieza" },
                },
                InlineFields = true,
                NumberOfReplicas = 1,
                NumberOfShards = 1,
                ConnectionTimeout = TimeSpan.FromSeconds(10),
                FailureCallback = e => Console.WriteLine($"Error de Elasticsearch: {e.Exception?.ToString()}"),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                  EmitEventFailureHandling.WriteToFailureSink |
                                  EmitEventFailureHandling.RaiseCallback,
                MinimumLogEventLevel = Serilog.Events.LogEventLevel.Debug,


                BufferBaseFilename = Path.Combine(Path.GetTempPath(), "elasticsearch-buffer"),
                BufferFileCountLimit = 5,
                BufferFileSizeLimitBytes = 10485760, // 10MB
                BufferLogShippingInterval = TimeSpan.FromSeconds(5),
                DetectElasticsearchVersion = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
            };

            // Solo aplicar autenticación si hay credenciales
            if (credentials != null && credentials.Length == 2)
            {
                options.ModifyConnectionSettings = conn =>
                conn.BasicAuthentication(credentials[0], credentials[1])
                      .RequestTimeout(TimeSpan.FromSeconds(30))
                      .DeadTimeout(TimeSpan.FromSeconds(60));
            }

            return options;
        }

        public class SerilogAdapter : ILog
        {
            private readonly ILogger _serilogLogger;

            public SerilogAdapter(ILogger serilogLogger)
            {
                _serilogLogger = serilogLogger;
            }

            public bool IsDebugEnabled => true;

            public void Debug(object message) => _serilogLogger.Debug("{Message}", message);
            public void Debug(object message, Exception exception) => _serilogLogger.Debug(exception, "{Message}", message);
            public void DebugFormat(string format, params object[] args) => _serilogLogger.Debug(format, args);
            public void Error(object message) => _serilogLogger.Error("{Message}", message);
            public void Error(object message, Exception exception) => _serilogLogger.Error(exception, "{Message}", message);
            public void ErrorFormat(string format, params object[] args) => _serilogLogger.Error(format, args);
            public void Fatal(object message) => _serilogLogger.Fatal("{Message}", message);
            public void Fatal(object message, Exception exception) => _serilogLogger.Fatal(exception, "{Message}", message);
            public void FatalFormat(string format, params object[] args) => _serilogLogger.Fatal(format, args);
            public void Info(object message) => _serilogLogger.Information("{Message}", message);
            public void Info(object message, Exception exception) => _serilogLogger.Information(exception, "{Message}", message);
            public void InfoFormat(string format, params object[] args) => _serilogLogger.Information(format, args);
            public void Warn(object message) => _serilogLogger.Warning("{Message}", message);
            public void Warn(object message, Exception exception) => _serilogLogger.Warning(exception, "{Message}", message);
            public void WarnFormat(string format, params object[] args) => _serilogLogger.Warning(format, args);
        }

        // Factory para crear loggers de ServiceStack usando Serilog
        public class SerilogFactory : ILogFactory
        {
            public ILog GetLogger(Type type) => new SerilogAdapter(Log.ForContext(type));
            public ILog GetLogger(string typeName) => new SerilogAdapter(Log.ForContext("SourceContext", typeName));
        }
    }

    public class MultiConditionalFilter : ILogEventFilter
    {
        private readonly List<(string PropertyName, Func<LogEventPropertyValue, bool> Condition, bool RequiredProperty)> _conditions;

        public MultiConditionalFilter()
        {
            _conditions = new List<(string, Func<LogEventPropertyValue, bool>, bool)>();
        }


        public MultiConditionalFilter AddCondition(string propertyName, Func<LogEventPropertyValue, bool> condition, bool requiredProperty = true)
        {
            _conditions.Add((propertyName, condition, requiredProperty));
            return this;
        }

        public bool IsEnabled(LogEvent logEvent)
        {
            if (_conditions.Count == 0)
                return true;

            foreach (var (propertyName, condition, requiredProperty) in _conditions)
            {
                if (logEvent.Properties.TryGetValue(propertyName, out var propValue))
                {
                    if (!condition(propValue))
                        return false;
                }
                else if (requiredProperty)
                {
                    return false;
                }
            }
            return true;
        }

        public static MultiConditionalFilter CondicionesLog()
        {
            var multiFilter = new MultiConditionalFilter()


            .AddCondition("message", value =>
            {
                return value != null;
            }, requiredProperty: false)

            .AddCondition("Path", value =>
            {
                string path = value.ToString().Trim('"').ToLower();
                return !path.Contains("/health") && !path.Contains("/metrics");
            }, requiredProperty: false)

            .AddCondition("Method", value =>
            {
                string method = value.ToString().Trim('"').ToUpper();
                return method == "POST" || method == "PUT" || method == "DELETE";
            }, requiredProperty: false);

            return multiFilter;
        }


        public static Func<LogEvent, bool> ExcluirMensajesNoDeseados()
        {
            string[] mensajesAExcluir = new[]
            {
                "request finished",
                "request starting",
                "increasing the maxrequestbodysize"
            };

            string[] sourceContextsAExcluir = new[]
            {
                "microsoft.hosting.lifetime",
                "administrador.interface.appadmservice"
            };

            return logEvent =>
            {
                var message = logEvent.MessageTemplate?.Text?.ToLowerInvariant();
                if (!string.IsNullOrEmpty(message) && mensajesAExcluir.Any(m => message.Contains(m)))
                    return true;

                if (logEvent.Properties.TryGetValue("SourceContext", out var sc))
                {
                    var sourceContext = sc.ToString().Trim('"').ToLowerInvariant();
                    if (sourceContextsAExcluir.Contains(sourceContext))
                        return true;
                }

                return false;
            };
        }
    }
}





