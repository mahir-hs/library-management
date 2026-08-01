using LibraryManagement.Application.DTOs.Auth;
using MediatR;

namespace LibraryManagement.Application.Features.Auth.Commands;

public record RegisterCommand(string Username, string Email, string Password, string FullName, string PhoneNumber)
    : IRequest<UserDto>;