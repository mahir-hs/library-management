using LibraryManagement.Application.DTOs.Auth;
using MediatR;

namespace LibraryManagement.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;