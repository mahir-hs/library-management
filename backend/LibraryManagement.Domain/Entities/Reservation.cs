namespace LibraryManagement.Domain.Entities;

using Common;
using Enums;

public sealed class Reservation : AuditableEntity
{
    public Guid MemberId { get; set; }
    public Guid BookId { get; set; }
    public int PositionInQueue { get; set; }
    public DateTimeOffset ReservedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset? FulfilledAt { get; set; }
    public ReservationStatus Status { get; set; }
    public Member Member { get; set; } = null!;
    public Book Book { get; set; } = null!;
}