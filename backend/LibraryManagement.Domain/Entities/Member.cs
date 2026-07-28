namespace LibraryManagement.Domain.Entities;

using Common;

public sealed class Member : AuditableEntity
{
    public Guid UserId { get; set; }
    public required string MembershipNumber { get; set; }
    public required string Address { get; set; }
    public DateTimeOffset JoinedDate { get; set; }
    public User User { get; set; } = null!;
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = [];
    public ICollection<Reservation> Reservations { get; set; } = [];
}