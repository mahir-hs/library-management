using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs.Reservation;

public class MyReservationsResponse
{
    public Guid Id { get; set; }
    public required string BookTitle { get; set; }
    public required string ISBN { get; set; }
    public required string AuthorName { get; set; }
    public required string CategoryName { get; set; }
    public int PositionInQueue { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime ReservedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}