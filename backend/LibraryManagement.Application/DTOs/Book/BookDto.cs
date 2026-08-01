namespace LibraryManagement.Application.DTOs.Book;

public class BookDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string ISBN { get; set; }
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public int? PublishedYear { get; set; }
    public string? Language { get; set; }
    public string? ImageUrl { get; set; }
    public required string AuthorName { get; set; }
    public required string CategoryName { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public DateTime CreatedAt { get; set; }
}
