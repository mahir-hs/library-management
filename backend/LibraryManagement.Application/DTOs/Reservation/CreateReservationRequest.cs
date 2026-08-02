namespace LibraryManagement.Application.DTOs.Reservation;

public class CreateReservationRequest
{
    public required Guid MemberId { get; set; }
    public required Guid BookId { get; set; }
}

public class FulfillReservationRequest
{
    public required Guid Id { get; set; }
    public required Guid BookCopyId { get; set; }
}