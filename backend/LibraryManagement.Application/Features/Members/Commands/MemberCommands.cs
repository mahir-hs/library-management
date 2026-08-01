using LibraryManagement.Application.DTOs.Member;
using MediatR;

namespace LibraryManagement.Application.Features.Members.Commands;

public record CreateMemberCommand(
    Guid UserId,
    string MembershipNumber,
    string Address,
    string? PhoneNumber
) : IRequest<MemberDetailDto>;

public record UpdateMemberCommand(
    Guid Id,
    string? MembershipNumber,
    string? Address,
    string? PhoneNumber
) : IRequest<MemberDto>;

public record DeleteMemberCommand(Guid Id) : IRequest<bool>;