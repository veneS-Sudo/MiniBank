using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace Minibank.Web.Infrastructure.Swagger
{
    public static class SwaggerConfigurations
    {
        public static IServiceCollection AddSwaggerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new OpenApiInfo { Title = "Minibank API", Version = "v1.0" });

                options.AddSecurityDefinition("oauth2",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            ClientCredentials = new OpenApiOAuthFlow
                            {
                                TokenUrl =
                                    new Uri(Environment.GetEnvironmentVariable("ISSUER_TOKEN") ??
                                            configuration.GetConnectionString("IssuerTokenUri")),
                                Scopes = new Dictionary<string, string>()
                            }
                        }
                    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = SecuritySchemeType.OAuth2.GetDisplayName()
                            }
                        },
                        new List<string>()
                    }
                });
            });
            
            return services;
        }
    }
}