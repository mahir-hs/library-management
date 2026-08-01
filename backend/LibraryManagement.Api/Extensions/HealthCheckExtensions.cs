using LibraryManagement.Infrastructure.Persistence.Context;

namespace LibraryManagement.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring health checks.
/// </summary>
public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(name: "database");

        return services;
    }
}
