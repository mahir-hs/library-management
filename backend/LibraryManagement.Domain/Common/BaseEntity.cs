namespace LibraryManagement.Domain.Common;

public abstract class BaseEntity : IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
}
