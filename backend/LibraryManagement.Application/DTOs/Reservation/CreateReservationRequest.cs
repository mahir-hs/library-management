namespace LibraryManagement.Application.DTOs.Reservation;

public class CreateReservationRequest
{
    public required Guid MemberId { get; set; }
    public required Guid BookId { get; set; }
}