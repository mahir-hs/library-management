namespace LibraryManagement.Application.DTOs.Book;

public class CreateBookRequest
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
}
