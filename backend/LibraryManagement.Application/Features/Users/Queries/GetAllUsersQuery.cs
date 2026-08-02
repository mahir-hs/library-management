using LibraryManagement.Application.DTOs.Auth;
using LibraryManagement.Domain.Common;
using MediatR;

namespace LibraryManagement.Application.Features.Users.Queries;

public record GetAllUsersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<UserDto>>;
