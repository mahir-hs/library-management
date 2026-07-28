namespace LibraryManagement.Domain.Entities;

using Common;

public sealed class Author : AuditableEntity
{
    public required string Name { get; set; }
    public string? Biography { get; set; }
    public ICollection<Book> Books { get; set; } = [];
}