using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace LibraryManagement.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation.
/// </summary>
public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Library Management API",
                Version = "v1",
                Description = "RESTful API for Library Management System",
                Contact = new OpenApiContact
                {
                    Name = "Library Management Team",
                    Email = "support@librarymgmt.com"
                }
            });

            // Define the Security Scheme
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
            });

            // Add Security Requirement using OpenApiSecuritySchemeReference
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });

            // Include XML comments from project assemblies
            var xmlFiles = new[] { "LibraryManagement.Domain.xml", "LibraryManagement.Application.xml", "LibraryManagement.Api.xml" };
            foreach (var xmlFile in xmlFiles)
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }
        });

        return services;
    }
}
