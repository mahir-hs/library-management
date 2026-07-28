namespace LibraryManagement.Domain.Entities;

using Common;

public sealed class Book : AuditableEntity
{
    public required string Title { get; set; }
    public required string ISBN { get; set; }
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public int? PublishedYear { get; set; }
    public string? Language { get; set; }
    public string? ImageUrl { get; set; }
    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }
    public Author Author { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<BookCopy> Copies { get; set; } = [];
    public ICollection<Reservation> Reservations { get; set; } = [];
}