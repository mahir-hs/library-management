namespace LibraryManagement.Application.DTOs.Book;

public class BookSearchResponse
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string ISBN { get; set; }
    public required string AuthorName { get; set; }
    public required string CategoryName { get; set; }
    public int AvailableCopies { get; set; }
}