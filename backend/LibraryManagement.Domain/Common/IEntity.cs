namespace LibraryManagement.Domain.Common;

public interface IEntity<TId>
{
    TId Id { get; }
}
