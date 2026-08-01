using LibraryManagement.Application.DTOs.Branch;
using MediatR;

namespace LibraryManagement.Application.Features.Branchs.Commands;

public record CreateBranchCommand(
    string Name,
    string Code,
    string Address,
    string? Phone,
    string? Email
) : IRequest<BranchDto>;

public record UpdateBranchCommand(
    Guid Id,
    string Name,
    string Code,
    string Address,
    string? Phone,
    string? Email,
    bool IsActive
) : IRequest<BranchDto>;

public record DeleteBranchCommand(Guid Id) : IRequest<bool>;
