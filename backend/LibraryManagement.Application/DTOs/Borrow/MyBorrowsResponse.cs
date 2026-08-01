using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs.Borrow;

public class MyBorrowsResponse
{
    public Guid Id { get; set; }
    public required string BookTitle { get; set; }
    public required string BookISBN { get; set; }
    public required string AuthorName { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public BorrowStatus Status { get; set; }
}