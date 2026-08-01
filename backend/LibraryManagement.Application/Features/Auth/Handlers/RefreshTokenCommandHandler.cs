using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.DTOs.Auth;
using LibraryManagement.Application.Features.Auth.Commands;
using MediatR;

namespace LibraryManagement.Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(IAuthService authService, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(
            request.AccessToken,
            request.RefreshToken,
            cancellationToken
        );

        if (result is null)
        {
            throw new UnauthorizedException("Invalid or expired refresh token");
        }

        var (newAccessToken, newRefreshToken) = result.Value;
        var userId = _authService.GetUserIdFromToken(newAccessToken);
        var role = _authService.GetUserRoleFromToken(newAccessToken);

        if (!userId.HasValue)
        {
            throw new UnauthorizedException("Failed to extract user information from token");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = role ?? user.Role.ToString(),
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
}