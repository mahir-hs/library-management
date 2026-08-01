namespace LibraryManagement.Application.DTOs.Book;

public class UpdateBookRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public int? PublishedYear { get; set; }
    public string? Language { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? CategoryId { get; set; }
}