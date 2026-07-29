using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Persistence.Context;
using LibraryManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagement.Infrastructure.Repositories;


/// <summary>
/// Unit of Work implementation - orchestrates multiple repositories and manages transactions
/// Ensures ACID properties when multiple entities are modified together
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private IRepository<User>? _users;
    private IRepository<Book>? _books;
    private IRepository<BookCopy>? _bookCopies;
    private IRepository<Member>? _members;
    private IRepository<BorrowRecord>? _borrowRecords;
    private IRepository<Reservation>? _reservations;
    private IRepository<Author>? _authors;
    private IRepository<Category>? _categories;
    private IRepository<RefreshToken>? _refreshTokens;
    private IRepository<Branch>? _branchs;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #region Repository Properties

    public IRepository<User> Users =>
        _users ??= (IRepository<User>)new GenericRepository<User>(_context);

    public IRepository<Book> Books =>
        _books ??= (IRepository<Book>)new GenericRepository<Book>(_context);

    public IRepository<BookCopy> BookCopies =>
        _bookCopies ??= (IRepository<BookCopy>)new GenericRepository<BookCopy>(_context);

    public IRepository<Member> Members =>
        _members ??= (IRepository<Member>)new GenericRepository<Member>(_context);

    public IRepository<BorrowRecord> BorrowRecords =>
        _borrowRecords ??= (IRepository<BorrowRecord>)new GenericRepository<BorrowRecord>(_context);

    public IRepository<Reservation> Reservations =>
        _reservations ??= (IRepository<Reservation>)new GenericRepository<Reservation>(_context);

    public IRepository<Author> Authors =>
        _authors ??= (IRepository<Author>)new GenericRepository<Author>(_context);

    public IRepository<Category> Categories =>
        _categories ??= (IRepository<Category>)new GenericRepository<Category>(_context);

    public IRepository<RefreshToken> RefreshTokens =>
        _refreshTokens ??= (IRepository<RefreshToken>)new GenericRepository<RefreshToken>(_context);

    public IRepository<Branch> Branchs =>
       _branchs ??= (IRepository<Branch>)new GenericRepository<Branch>(_context);

    #endregion

    #region Transaction Management

    /// <summary>
    /// Begin a new database transaction
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commit the current transaction
    /// Persists all pending changes to the database
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    /// <summary>
    /// Rollback the current transaction
    /// Discards all pending changes
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    #endregion

    #region Save Changes

    /// <summary>
    /// Save all pending changes to the database
    /// Without an explicit transaction, changes are auto-committed
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Disposal

    /// <summary>
    /// Async disposal - cleans up transaction and context
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        if (_context != null)
        {
            await _context.DisposeAsync();
        }
    }

    #endregion
}
