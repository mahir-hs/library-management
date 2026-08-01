using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs.Borrow;

public class BorrowListResponse
{
    public Guid Id { get; set; }
    public required string MemberName { get; set; }
    public required string BookTitle { get; set; }
    public required string BookISBN { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public BorrowStatus Status { get; set; }
    public int DaysOverdue { get; set; }
}