using FluentValidation;
using LibraryManagement.Application.DTOs.Borrow;

namespace LibraryManagement.Application.Validators.Borrow;

public class CreateBorrowRequestValidator : AbstractValidator<CreateBorrowRequest>
{
    public CreateBorrowRequestValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("Member ID is required");

        RuleFor(x => x.BookCopyId)
            .NotEmpty().WithMessage("Book copy ID is required");
    }
}