namespace LibraryManagement.Api.Extensions;

using Application.Common.Behaviors;
using Application.Features.Auth.Commands;
using Application.Validators.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering Application layer services
/// </summary>
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);

            // Register pipeline behaviors (order matters: logging → validation → handler)
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        return services;
    }
}
