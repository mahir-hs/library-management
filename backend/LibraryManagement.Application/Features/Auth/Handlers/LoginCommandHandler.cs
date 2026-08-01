using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Auth;
using LibraryManagement.Application.Features.Auth.Commands;
using LibraryManagement.Domain.Entities;
using MediatR;

namespace LibraryManagement.Application.Features.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(IAuthService authService, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

        if (result is null)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        var (accessToken, refreshToken) = result.Value;

        // Get user details
        var userSpec = new UserByEmailSpecification(request.Email);
        var user = await _unitOfWork.Users.GetFirstAsync(userSpec, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", request.Email);
        }

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
}

// Specification for finding user by email
public class UserByEmailSpecification : SpecificationBase<User>
{
    public UserByEmailSpecification(string email)
    {
        Criteria = u => u.Email.ToLower() == email.ToLower();
    }
}
