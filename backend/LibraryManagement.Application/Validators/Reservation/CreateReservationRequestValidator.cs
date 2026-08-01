using FluentValidation;
using LibraryManagement.Application.DTOs.Reservation;

namespace LibraryManagement.Application.Validators.Reservation;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Book ID is required");
    }
}