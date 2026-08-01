using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs.Reservation;

public class ReservationQueueDto
{
    public Guid Id { get; set; }
    public required string MemberName { get; set; }
    public required string BookTitle { get; set; }
    public required string ISBN { get; set; }
    public int PositionInQueue { get; set; }
    public DateTime ReservedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public ReservationStatus Status { get; set; }
}