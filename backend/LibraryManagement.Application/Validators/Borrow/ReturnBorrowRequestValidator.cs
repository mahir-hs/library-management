using FluentValidation;
using LibraryManagement.Application.DTOs.Borrow;

namespace LibraryManagement.Application.Validators.Borrow;

public class ReturnBorrowRequestValidator : AbstractValidator<ReturnBorrowRequest>
{
    public ReturnBorrowRequestValidator()
    {
        RuleFor(x => x.FineAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Fine amount cannot be negative")
            .When(x => x.FineAmount.HasValue);
    }
}