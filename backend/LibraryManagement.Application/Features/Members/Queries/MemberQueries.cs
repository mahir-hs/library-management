using LibraryManagement.Application.DTOs.Member;
using LibraryManagement.Domain.Common;
using MediatR;

namespace LibraryManagement.Application.Features.Members.Queries;

public record GetMemberByIdQuery(Guid Id) : IRequest<MemberDetailDto?>;

public record GetMemberByUserIdQuery(Guid UserId) : IRequest<MemberDetailDto?>;

public record GetAllMembersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<MemberDto>>;