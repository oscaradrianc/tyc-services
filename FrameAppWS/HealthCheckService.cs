// HealthCheckService.cs
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;

[Route("/health", "GET")]
[Route("/health/live", "GET")]
public class HealthCheckRQ : IReturn<HealthCheckRS> { }

[Route("/health/ready", "GET")]
public class ReadinessCheckRQ : IReturn<HealthCheckRS> { }

public class HealthCheckRS
{
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string Version { get; set; }
    public Dictionary<string, object> Details { get; set; }
}

public class HealthCheckService : Service
{
    public IRedisClientsManager RedisManager { get; set; }

    public object Get(HealthCheckRQ request)
    {
        return new HealthCheckRS
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = GetType().Assembly.GetName().Version?.ToString()
        };
    }

    public object Get(ReadinessCheckRQ request)
    {
        var details = new Dictionary<string, object>();
        var isHealthy = true;

        // Check Redis
        try
        {
            using var client = RedisManager.GetClient();
            client.Ping();
            details["redis"] = "OK";
        }
        catch (Exception ex)
        {
            details["redis"] = $"FAIL: {ex.Message}";
            isHealthy = false;
        }

        // Check DB (ejemplo básico)
        try
        {
            using var db = HostContext.AppHost.GetDbConnection();
            db.ExecuteSql("SELECT 1");
            details["database"] = "OK";
        }
        catch (Exception ex)
        {
            details["database"] = $"FAIL: {ex.Message}";
            isHealthy = false;
        }

        return new HealthCheckRS
        {
            Status = isHealthy ? "Healthy" : "Unhealthy",
            Timestamp = DateTime.UtcNow,
            Version = GetType().Assembly.GetName().Version?.ToString(),
            Details = details
        };
    }
}