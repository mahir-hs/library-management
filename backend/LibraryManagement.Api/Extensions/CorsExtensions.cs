namespace LibraryManagement.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring CORS policies.
/// </summary>
public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:4200",      // Angular dev server
                        "http://localhost:3000",      // Alternative port
                        "http://127.0.0.1:4200")      // 127.0.0.1 variant
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
