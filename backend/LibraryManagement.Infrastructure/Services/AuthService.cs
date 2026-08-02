namespace LibraryManagement.Infrastructure.Services;

using Domain.Entities;
using Domain.Enums;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<(string AccessToken, string RefreshToken)?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var spec = new UserByEmailSpecification(email);
        var user = await _unitOfWork.Users.GetFirstAsync(spec, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        // Update last login
        user.LastLoginAt = DateTimeOffset.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
        };
        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (accessToken, refreshToken);
    }

    public async Task<Guid> RegisterAsync(
        string username,
        string email,
        string password,
        string fullName,
        string phoneNumber,
        Guid branchId,
        string role,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<UserRole>(role, ignoreCase: true, out var userRole))
        {
            userRole = UserRole.Member;
        }

        var user = new User
        {
            Username = username,
            Email = email,
            FullName = fullName,
            Role = userRole,
            PhoneNumber = phoneNumber,
            BranchId = branchId,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(
        string accessToken,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (!_tokenService.ValidateToken(accessToken))
        {
            return null;
        }

        var userId = _tokenService.GetUserIdFromToken(accessToken);
        if (!userId.HasValue)
        {
            return null;
        }

        var spec = new RefreshTokenSpecification(userId.Value, refreshToken);
        var storedToken = await _unitOfWork.RefreshTokens.GetFirstAsync(spec, cancellationToken);

        if (storedToken == null || storedToken.IsExpired || storedToken.IsRevoked)
        {
            return null;
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Revoke old refresh token
        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        await _unitOfWork.RefreshTokens.UpdateAsync(storedToken, cancellationToken);

        // Add new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
        };
        await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (newAccessToken, newRefreshToken);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var spec = new RefreshTokenByTokenSpecification(refreshToken);
        var token = await _unitOfWork.RefreshTokens.GetFirstAsync(spec, cancellationToken);

        if (token != null)
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
            await _unitOfWork.RefreshTokens.UpdateAsync(token, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> VerifyTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return _tokenService.ValidateToken(token);
    }

    public Guid? GetUserIdFromToken(string token)
    {
        return _tokenService.GetUserIdFromToken(token);
    }

    public string? GetUserRoleFromToken(string token)
    {
        return _tokenService.GetUserRoleFromToken(token);
    }
}

public class UserByEmailSpecification : SpecificationBase<User>
{
    public UserByEmailSpecification(string email)
    {
        Criteria = u => u.Email.ToLower() == email.ToLower();
    }
}

public class RefreshTokenSpecification : SpecificationBase<RefreshToken>
{
    public RefreshTokenSpecification(Guid userId, string token)
    {
        Criteria = rt => rt.UserId == userId && rt.Token == token;
    }
}

public class RefreshTokenByTokenSpecification : SpecificationBase<RefreshToken>
{
    public RefreshTokenByTokenSpecification(string token)
    {
        Criteria = rt => rt.Token == token;
    }
}