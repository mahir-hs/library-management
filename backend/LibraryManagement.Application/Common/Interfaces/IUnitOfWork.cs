using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Common.Interfaces;

/// <summary>
/// Unit of Work interface - manages transactions across multiple repositories
/// Ensures consistency when multiple entities are modified together
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Repository for User entities
    /// </summary>
    IRepository<User> Users { get; }

    /// <summary>
    /// Repository for Book entities
    /// </summary>
    IRepository<Book> Books { get; }

    /// <summary>
    /// Repository for BookCopy entities
    /// </summary>
    IRepository<BookCopy> BookCopies { get; }

    /// <summary>
    /// Repository for Member entities
    /// </summary>
    IRepository<Member> Members { get; }

    /// <summary>
    /// Repository for BorrowRecord entities
    /// </summary>
    IRepository<BorrowRecord> BorrowRecords { get; }

    /// <summary>
    /// Repository for Reservation entities
    /// </summary>
    IRepository<Reservation> Reservations { get; }

    /// <summary>
    /// Repository for Author entities
    /// </summary>
    IRepository<Author> Authors { get; }

    /// <summary>
    /// Repository for Category entities
    /// </summary>
    IRepository<Category> Categories { get; }

    /// <summary>
    /// Repository for RefreshToken entities
    /// </summary>
    IRepository<RefreshToken> RefreshTokens { get; }

    /// <summary>
    /// Repository for Branch entities
    /// </summary>
    IRepository<Branch> Branchs { get; }

    /// <summary>
    /// Save all pending changes to the database
    /// Must be called after modifying any entities
    /// </summary>
    /// <returns>Number of entities affected</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin a new database transaction
    /// All changes are rolled back if SaveChangesAsync is not called
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction
    /// All changes are persisted to the database
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction
    /// All changes made since BeginTransactionAsync are discarded
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
