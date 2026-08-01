using FluentValidation;
using LibraryManagement.Application.DTOs.Book;

namespace LibraryManagement.Application.Validators.Book;

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.PublishedYear)
            .InclusiveBetween(1000, 2100).WithMessage("Published year must be between 1000 and 2100")
            .When(x => x.PublishedYear.HasValue);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required")
            .When(x => x.CategoryId.HasValue);
    }
}