using LibraryManagement.Application.DTOs.Branch;
using LibraryManagement.Domain.Common;
using MediatR;

namespace LibraryManagement.Application.Features.Branchs.Queries;

public record GetBranchByIdQuery(Guid Id) : IRequest<BranchDto?>;

public record GetBranchByCodeQuery(string Code) : IRequest<BranchDto?>;

public record GetAllBranchesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<BranchDto>>;

public record SearchBranchesQuery(
    string? Name = null,
    string? Code = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedResult<BranchSearchResponse>>;
