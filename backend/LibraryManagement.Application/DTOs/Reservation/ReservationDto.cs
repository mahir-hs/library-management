using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs.Reservation;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public required string MemberName { get; set; }
    public Guid BookId { get; set; }
    public required string BookTitle { get; set; }
    public required string ISBN { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime ReservedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
