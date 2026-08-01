namespace LibraryManagement.Application.Common.Interfaces;

using Common.Specifications;

/// <summary>
/// Generic repository interface for data access
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities using a specification
    /// </summary>
    Task<IReadOnlyList<T>> GetAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets first entity matching specification or null
    /// </summary>
    Task<T?> GetFirstAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets count of entities matching specification
    /// </summary>
    Task<int> CountAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches specification
    /// </summary>
    Task<bool> AnyAsync(SpecificationBase<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple entities
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an entity with given ID exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}