using FluentValidation;
using LibraryManagement.Application.DTOs.Member;

namespace LibraryManagement.Application.Validators.Member;

public class UpdateMemberRequestValidator : AbstractValidator<UpdateMemberRequest>
{
    public UpdateMemberRequestValidator()
    {
        RuleFor(x => x.MembershipNumber)
            .MinimumLength(3).WithMessage("Membership number must be at least 3 characters")
            .MaximumLength(50).WithMessage("Membership number must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.MembershipNumber));

        RuleFor(x => x.Address)
            .MinimumLength(5).WithMessage("Address must be at least 5 characters")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Phone number must be valid")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
