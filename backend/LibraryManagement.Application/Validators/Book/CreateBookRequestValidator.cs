using FluentValidation;
using LibraryManagement.Application.DTOs.Book;

namespace LibraryManagement.Application.Validators.Book;

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required")
            .MaximumLength(13).WithMessage("ISBN must not exceed 13 characters");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required");

        RuleFor(x => x.PublishedYear)
            .InclusiveBetween(1000, 2100).WithMessage("Published year must be between 1000 and 2100")
            .When(x => x.PublishedYear.HasValue);
    }
}