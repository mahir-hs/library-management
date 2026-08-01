namespace LibraryManagement.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring authorization policies.
/// </summary>
public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("AdminOrLibrarian", policy =>
                policy.RequireRole("Admin", "Librarian"));

            options.AddPolicy("MemberOnly", policy =>
                policy.RequireRole("Member"));

            options.AddPolicy("AnyAuthenticated", policy =>
                policy.RequireRole("Admin", "Librarian", "Member"));
        });

        return services;
    }
}
