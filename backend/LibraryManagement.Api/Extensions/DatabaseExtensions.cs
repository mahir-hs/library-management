using LibraryManagement.Infrastructure.Persistence.Context;
using LibraryManagement.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LibraryManagement.Api.Extensions;

/// <summary>
/// Extension methods for initializing the database (migrations and seeding).
/// </summary>
public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Domain.Entities.User>>();
        try
        {
            Log.Information("Applying database migrations...");
            await dbContext.Database.MigrateAsync();
            Log.Information("Migrations applied.");

            Log.Information("Seeding database if not already seeded...");
            await SeedData.SeedDatabaseAsync(dbContext, passwordHasher);
            Log.Information("Database seeding complete.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database initialization failed. Ensure database connection string is correct.");
        }
    }
}
