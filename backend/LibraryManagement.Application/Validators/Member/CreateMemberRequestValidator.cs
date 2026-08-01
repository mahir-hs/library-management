namespace LibraryManagement.Application.Validators.Member;

using DTOs.Member;
using FluentValidation;

public class CreateMemberRequestValidator : AbstractValidator<CreateMemberRequest>
{
    public CreateMemberRequestValidator()
    {
        RuleFor(x => x.MembershipNumber)
            .NotEmpty().WithMessage("Membership number is required")
            .MinimumLength(3).WithMessage("Membership number must be at least 3 characters")
            .MaximumLength(50).WithMessage("Membership number must not exceed 50 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MinimumLength(5).WithMessage("Address must be at least 5 characters")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters");
    }
}