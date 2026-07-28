namespace LibraryManagement.Domain.Entities;

using Common;
using Enums;

public sealed class BorrowRecord : AuditableEntity
{
    public Guid MemberId { get; set; }
    public Guid BookCopyId { get; set; }
    public DateTimeOffset BorrowedAt { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public DateTimeOffset? ReturnedAt { get; set; }
    public BorrowStatus Status { get; set; }
    public decimal FineAmount { get; set; }
    public Member Member { get; set; } = null!;
    public BookCopy BookCopy { get; set; } = null!;
}