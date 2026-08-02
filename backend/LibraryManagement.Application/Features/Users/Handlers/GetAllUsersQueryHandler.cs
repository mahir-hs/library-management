using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Auth;
using LibraryManagement.Application.Features.Users.Queries;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Entities;
using MediatR;

namespace LibraryManagement.Application.Features.Users.Handlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResult<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAllUsersSpecification(request.PageNumber, request.PageSize);
        var users = await _unitOfWork.Users.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.Users.CountAsync(new GetAllUsersSpecification(), cancellationToken);

        var userDtos = users.Select(user => new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt?.DateTime
        }).ToList();

        return new PaginatedResult<UserDto>
        {
            Items = userDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}
