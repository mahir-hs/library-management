namespace LibraryManagement.Domain.Entities;

using Common;

public sealed class Category : AuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Book> Books { get; set; } = [];
}