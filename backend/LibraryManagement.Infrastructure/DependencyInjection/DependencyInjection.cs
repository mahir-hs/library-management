namespace LibraryManagement.Infrastructure.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Infrastructure.Persistence.Interceptors;
using LibraryManagement.Infrastructure.Services;
using LibraryManagement.Infrastructure.Persistence.Context;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Required for reading HttpContext in CurrentUserService
        services.AddHttpContextAccessor();

        // 2. Register CurrentUserService
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // 3. Register the Interceptor
        services.AddScoped<AuditableEntityInterceptor>();

        // 4. Register DbContext with the Interceptor attached
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var interceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();

            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(interceptor);
        });

        return services;
    }
}