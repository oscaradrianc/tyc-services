using Microsoft.AspNetCore.Builder;

namespace FrameAppWS.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseDynamicCors(
        this IApplicationBuilder builder,
        string baseDomain = "midominio.com")
    {
        return builder.UseMiddleware<CorsMiddleware>(baseDomain);
    }
}