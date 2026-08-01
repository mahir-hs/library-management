namespace LibraryManagement.Domain.Entities;

using Common;
using Enums;

public sealed class BookCopy : AuditableEntity
{
    public Guid BookId { get; set; }
    public required string Barcode { get; set; }
    public BookCopyStatus Status { get; set; }
    public required string ShelfLocation { get; set; }
    public DateTimeOffset AcquiredDate { get; set; }
    public Guid? BranchId { get; set; }
    public Book Book { get; set; } = null!;
    public Branch? Branch { get; set; }
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = [];
}