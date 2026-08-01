namespace LibraryManagement.Application.DTOs.Book;

public class AddBookCopiesRequest
{
    public int Quantity { get; set; }
    public Guid BranchId { get; set; }
    public required string ShelfLocation { get; set; }
}
