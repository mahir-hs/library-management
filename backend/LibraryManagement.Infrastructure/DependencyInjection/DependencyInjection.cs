namespace LibraryManagement.Infrastructure.DependencyInjection;

using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Infrastructure.Persistence.Context;
using LibraryManagement.Infrastructure.Persistence.Interceptors;
using LibraryManagement.Infrastructure.Persistence.Repositories;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var interceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();

            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(interceptor)
                   .UseSnakeCaseNamingConvention()
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        });

        return services;
    }
}



