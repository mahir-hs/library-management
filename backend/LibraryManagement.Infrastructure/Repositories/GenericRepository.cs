using LibraryManagement.Domain.Common;
using LibraryManagement.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class GenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    #region Read Operations

    /// <summary>
    /// Get a single entity by its ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Entity if found, null otherwise</returns>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync([id], cancellationToken: CancellationToken.None);
    }

    /// <summary>
    /// Get a single entity by ID with eager loading of related entities
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="includes">Related entity properties to include (e.g., b => b.Author)</param>
    /// <returns>Entity with related entities loaded, null if not found</returns>
    public virtual async Task<T?> GetByIdAsync(Guid id, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>List of all entities (subject to soft delete filters)</returns>
    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Get all entities with eager loading
    /// </summary>
    /// <param name="includes">Related entity properties to include</param>
    /// <returns>List of all entities with relationships loaded</returns>
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    /// <summary>
    /// Get entities matching a predicate (filter condition)
    /// </summary>
    /// <param name="predicate">Filter condition (e.g., b => b.Title.Contains("Harry"))</param>
    /// <returns>Matching entities</returns>
    public virtual async Task<IReadOnlyList<T>> GetByPredicateAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Get entities matching a predicate with eager loading
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related entity properties to include</param>
    /// <returns>Matching entities with relationships loaded</returns>
    public virtual async Task<IReadOnlyList<T>> GetByPredicateAsync(
        System.Linq.Expressions.Expression<Func<T, bool>> predicate,
        params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(predicate);

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    /// <summary>
    /// Get a single entity matching a predicate (returns first match or null)
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <returns>First matching entity or null</returns>
    public virtual async Task<T?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Check if any entity matches the predicate
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <returns>True if any entity matches</returns>
    public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    /// <summary>
    /// Count entities matching a predicate
    /// </summary>
    /// <param name="predicate">Filter condition (optional, null counts all)</param>
    /// <returns>Number of matching entities</returns>
    public virtual async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null)
    {
        return predicate == null
            ? await _dbSet.CountAsync()
            : await _dbSet.CountAsync(predicate);
    }

    #endregion

    #region Write Operations

    /// <summary>
    /// Add a single entity to the database (does NOT save changes)
    /// </summary>
    /// <param name="entity">Entity to add</param>
    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <summary>
    /// Add multiple entities to the database (does NOT save changes)
    /// </summary>
    /// <param name="entities">Entities to add</param>
    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    /// <summary>
    /// Update an entity in the database (does NOT save changes)
    /// </summary>
    /// <param name="entity">Entity to update (must be tracked or explicitly set)</param>
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Update multiple entities (does NOT save changes)
    /// </summary>
    /// <param name="entities">Entities to update</param>
    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    /// <summary>
    /// Delete an entity from the database (does NOT save changes)
    /// For AuditableEntity, this performs soft delete (sets DeletedAt)
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Delete multiple entities (does NOT save changes)
    /// </summary>
    /// <param name="entities">Entities to delete</param>
    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    #endregion

    #region Pagination

    /// <summary>
    /// Get paginated results
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="predicate">Optional filter condition</param>
    /// <returns>Paginated result with items and total count</returns>
    public virtual async Task<PaginatedResult<T>> GetPaginatedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;  // Max 100 per page

        IQueryable<T> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    #endregion

    #region Transaction Support

    /// <summary>
    /// Save all pending changes to the database
    /// </summary>
    /// <returns>Number of entities affected</returns>
    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    #endregion
}
