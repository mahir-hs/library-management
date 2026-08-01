using LibraryManagement.Application.DTOs.Auth;
using MediatR;

namespace LibraryManagement.Application.Features.Users.Queries;

public record GetCurrentUserQuery : IRequest<UserDto?>;
