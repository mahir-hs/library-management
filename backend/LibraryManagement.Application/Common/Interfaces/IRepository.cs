using LibraryManagement.Domain.Common;

namespace LibraryManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository interface - contract for all repository implementations
/// Provides CRUD operations and filtering capabilities
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    #region Read Operations

    /// <summary>
    /// Get a single entity by its ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get a single entity by ID with related entities eager-loaded
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, params System.Linq.Expressions.Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>
    /// Get all entities with eager loading of related entities
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(params System.Linq.Expressions.Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Get entities matching a predicate
    /// </summary>
    Task<IReadOnlyList<T>> GetByPredicateAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Get entities matching a predicate with eager loading
    /// </summary>
    Task<IReadOnlyList<T>> GetByPredicateAsync(
        System.Linq.Expressions.Expression<Func<T, bool>> predicate,
        params System.Linq.Expressions.Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Get first entity matching a predicate, or null
    /// </summary>
    Task<T?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Check if any entity matches a predicate
    /// </summary>
    Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Count entities matching a predicate
    /// </summary>
    Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null);

    #endregion

    #region Write Operations

    /// <summary>
    /// Add a single entity (changes not persisted until SaveChangesAsync is called)
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Add multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Update an entity
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Update multiple entities
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Delete an entity (soft delete for AuditableEntity)
    /// </summary>
    void Delete(T entity);

    /// <summary>
    /// Delete multiple entities
    /// </summary>
    void DeleteRange(IEnumerable<T> entities);

    #endregion

    #region Pagination

    /// <summary>
    /// Get paginated results
    /// </summary>
    Task<PaginatedResult<T>> GetPaginatedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null);

    #endregion

    #region Save

    /// <summary>
    /// Persist all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync();

    #endregion
}