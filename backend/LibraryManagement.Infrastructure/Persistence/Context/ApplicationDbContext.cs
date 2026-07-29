namespace LibraryManagement.Infrastructure.Persistence.Context;

using Microsoft.EntityFrameworkCore;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #region DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCopy> BookCopies => Set<BookCopy>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    #endregion

    #region DbContext Overrides

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly,
            type => type.Namespace?.Contains("Configurations") ?? false
        );

        ConfigureAuditableEntities(modelBuilder);

        ConfigureEnumConversions(modelBuilder);
    }

    private static void ConfigureAuditableEntities(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Soft delete query filter
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(GetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                var filter = method.Invoke(null, Array.Empty<object>())!;
                entityType.SetQueryFilter((System.Linq.Expressions.LambdaExpression)filter);
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression GetSoftDeleteFilter<T>()
        where T : AuditableEntity
    {
        System.Linq.Expressions.Expression<Func<T, bool>> filter = x => x.DeletedAt == null;
        return filter;
    }

    private static void ConfigureEnumConversions(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsEnum)
                {
                    property.SetValueConverter(
                        property.ClrType.Name switch
                        {
                            nameof(Domain.Enums.UserRole) => new Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToNumberConverter<Domain.Enums.UserRole, int>(),
                            nameof(Domain.Enums.BookCopyStatus) => new Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToNumberConverter<Domain.Enums.BookCopyStatus, int>(),
                            nameof(Domain.Enums.BorrowStatus) => new Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToNumberConverter<Domain.Enums.BorrowStatus, int>(),
                            nameof(Domain.Enums.ReservationStatus) => new Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToNumberConverter<Domain.Enums.ReservationStatus, int>(),
                            _ => null
                        }
                    );
                }
            }
        }
    }

    #endregion
}