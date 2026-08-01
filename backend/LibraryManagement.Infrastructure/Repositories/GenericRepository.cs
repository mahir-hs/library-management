using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Domain.Common;
using LibraryManagement.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
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
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Get a single entity by ID with eager loading of related entities
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="includes">Related entity properties to include (e.g., b => b.Author)</param>
    /// <returns>Entity with related entities loaded, null if not found</returns>
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>List of all entities (subject to soft delete filters)</returns>
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get all entities with eager loading
    /// </summary>
    /// <param name="includes">Related entity properties to include</param>
    /// <returns>List of all entities with relationships loaded</returns>
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get entities matching a predicate (filter condition)
    /// </summary>
    /// <param name="predicate">Filter condition (e.g., b => b.Title.Contains("Harry"))</param>
    /// <returns>Matching entities</returns>
    public virtual async Task<IReadOnlyList<T>> GetByPredicateAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get entities matching a predicate with eager loading
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related entity properties to include</param>
    /// <returns>Matching entities with relationships loaded</returns>
    public virtual async Task<IReadOnlyList<T>> GetByPredicateAsync(
        System.Linq.Expressions.Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(predicate);

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get a single entity matching a predicate (returns first match or null)
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <returns>First matching entity or null</returns>
    public virtual async Task<T?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Check if any entity matches the predicate
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <returns>True if any entity matches</returns>
    public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Count entities matching a predicate
    /// </summary>
    /// <param name="predicate">Filter condition (optional, null counts all)</param>
    /// <returns>Number of matching entities</returns>
    public virtual async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);
    }

    #endregion

    #region Write Operations

    /// <summary>
    /// Add a single entity to the database (does NOT save changes)
    /// </summary>
    /// <param name="entity">Entity to add</param>
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Add multiple entities to the database (does NOT save changes)
    /// </summary>
    /// <param name="entities">Entities to add</param>
    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    /// <summary>
    /// Update an entity in the database (does NOT save changes)
    /// </summary>
    /// <param name="entity">Entity to update (must be tracked or explicitly set)</param>
    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Update multiple entities (does NOT save changes)
    /// </summary>
    /// <param name="entities">Entities to update</param>
    public virtual Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Delete an entity from the database (does NOT save changes)
    /// For AuditableEntity, this performs soft delete (sets DeletedAt)
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Delete multiple entities (does NOT save changes)
    /// </summary>
    /// <param name="entities">Entities to delete</param>
    public virtual Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    #endregion

    #region Specification-based Operations

    public virtual async Task<IReadOnlyList<T>> GetAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        foreach (var include in specification.Includes)
        {
            query = query.Include(include);
        }

        foreach (var includeString in specification.IncludeStrings)
        {
            query = query.Include(includeString);
        }

        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetFirstAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        foreach (var include in specification.Includes)
        {
            query = query.Include(include);
        }

        foreach (var includeString in specification.IncludeStrings)
        {
            query = query.Include(includeString);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        return await query.CountAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        return await query.AnyAsync(cancellationToken);
    }

    #endregion

    #region Existence

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
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
