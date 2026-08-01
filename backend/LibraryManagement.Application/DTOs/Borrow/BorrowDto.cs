using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs.Borrow;

public class BorrowDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public required string MemberName { get; set; }
    public Guid BookCopyId { get; set; }
    public required string BookTitle { get; set; }
    public required string BookISBN { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public BorrowStatus Status { get; set; }
    public decimal FineAmount { get; set; }
    public int DaysOverdue { get; set; }
}
