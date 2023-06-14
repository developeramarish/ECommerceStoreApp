﻿using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LocalizationService.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthenticationConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            var identityServerUrl = configuration.GetSection("IdentityServerConfigurations:Url").Value;
            var identityServerAudience = configuration.GetSection("IdentityServerConfigurations:Audience").Value;

            var staticIssuer = configuration.GetSection("StaticConfigurations:Issuer").Value;
            var staticAudience = configuration.GetSection("StaticConfigurations:Audience").Value;
            var staticScheme = configuration.GetSection("StaticConfigurations:Scheme").Value;
            var staticSecretKey = configuration.GetSection("StaticConfigurations:SecretKey").Value;

            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = identityServerUrl; // IdentityServer
                    options.Audience = identityServerAudience; // IdentityServer
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddJwtBearer(staticScheme, options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidIssuer = staticIssuer,
                        ValidAudience = staticAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(staticSecretKey)),

                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
    }
}
