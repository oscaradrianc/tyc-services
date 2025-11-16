using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Tyc.ws.Api.Security;

internal static class AuthJwtConfiguration
{
    public static IServiceCollection AddJwtConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenHandlers.Clear();
               options.TokenHandlers.Add(new JwtSecurityTokenHandler());
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidIssuer = configuration["Issuer"],
                   ValidateAudience = true,
                   ValidAudience = configuration["Audience"],
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["jwt.AuthKeyBase64"] ?? ""))
               };
               options.ConfigurationManager = null;
           });
        return services;
    }
}
