using LibraryManagement.Application.DTOs.Auth;
using MediatR;

namespace LibraryManagement.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken)
    : IRequest<AuthResponse>;
